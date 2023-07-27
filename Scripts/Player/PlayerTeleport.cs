using System;
using System.Collections;
using Chronos;
using Language.Lua;
using QFX.IFX;
using Rewired.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using MoreMountains.Feedbacks;

namespace ChromaShift.Scripts.Player
{
    public class PlayerTeleport : MonoBehaviour, PlayerTeleportInterface
    {
        public float BatteryChargeRequired { get; set; }
        [SerializeField] private ILifeSystem playerLifeSystem;

        public ILifeSystem PlayerLifeSystem
        {
            get => playerLifeSystem;
            set
            {
                playerLifeSystem = value;
            }
        }

        [ShowInInspector]
        public float Distance { get; set; }
        [ShowInInspector]
        public float CoolDownTime { get; set; }
        
        public float MaxScreenHeight { private get; set; }
        public float MinScreenHeight { private get; set; }

        private PlayerShip _playerShip;
        private TeleportFX _teleportFX;
        private Collider _collider;
        private RaycastHit _hit;
        private RaycastHit[] _hits;
        private bool _isHit;

        private float _teleportSpeed;
        private bool _hasForwardTeleport;
        private float _maxVerticalDistance;
        private int _mask;
        private Vector3 _size;
        private Vector3 _targetVector;
        
        public bool isTeleportEnabled;
        
        public float teleportMaxScreenHeight = 71.1f;
        public float teleportMinScreenHeight = -22.1f;
        
        public float teleportChargeRequirement = 33.3f;
        public float teleportDistance = 10f;
        public float teleportCooldownTime = 1f;
        
        
        //Cooldown System
        [ShowInInspector]
        public bool isLockedOut;
        [ShowInInspector]
        private float _activationsIndex;

        private  int _lockoutThreshold = 2;
        public float cooldownPenalty = 0.5f;
        public float lockoutPenalty = 5f;
        
        
        //UI
        public GameObject ui;
        public EnergyBar eBar;
        
        [Header("FEEDBACKS")] 
        [SerializeField] private MMFeedbacks _mmfTeleportActivate;
        
        public delegate void AbilityRightHandler();
        public event AbilityRightHandler UsedRightAbility;

        private void Awake()
        {
            _playerShip = gameObject.GetComponent<PlayerShip>();
            _playerShip.AbilityRight = this;
            _playerShip.teleportAbility = this;
            PlayerLifeSystem = _playerShip.LS;
            BatteryChargeRequired = teleportChargeRequirement;
            Distance = teleportDistance;
            CoolDownTime = teleportCooldownTime;
        }

        private void Start()
        {
            isLockedOut = false;
            _activationsIndex = 0;
            CoolDownTime = 0.0f;
            _maxVerticalDistance = 20f;
            _collider = gameObject.GetComponent<Collider>();
            _size = _collider.bounds.size * 1.1f;
            _hits = new RaycastHit[5];
            _mask = LayerMask.GetMask("Enemy", "Obstacle", "Scene", "Global");
            
            // @TODO Need to get the state object that would determine that the forward teleport ability is available.
            _hasForwardTeleport = true;
        }

        protected void FixedUpdate()
        {
            teleportMaxScreenHeight = LaneManager.Instance.laneArray[LaneManager.Instance.currentAnchorLanes[0]].y;
            teleportMinScreenHeight = LaneManager.Instance.laneArray[LaneManager.Instance.currentAnchorLanes[1]].y;

            //Debug.Log("Lane Boundries: " + teleportMaxScreenHeight + " | " + teleportMinScreenHeight);
            if (isTeleportEnabled)
            {
                MaxScreenHeight = teleportMaxScreenHeight;
                MinScreenHeight = teleportMinScreenHeight;
            }
        }

        private Vector3 GetTargetPosition(Vector3 origin, Vector3 testSize, Vector3 direction, Quaternion rotation, float maxDistance, int mask)
        {
            var maxPosition = origin + direction * maxDistance;
            testSize *= 0.5f;

            if (!Physics.CheckBox(maxPosition, testSize, rotation, mask))
            {
                return maxPosition;
            }
            
            var pos = origin;
            var size = Physics.BoxCastNonAlloc(origin, testSize, direction, _hits, rotation, maxDistance, mask);

            for (var i = 0; i < size; i++)
            {
                var hit = _hits[i];
                var testPosCloseToShip = origin + direction * hit.distance;

                var farDistance = direction * hit.distance + hit.collider.bounds.size.y * direction;
                var testPosFarSideOfHit = origin + farDistance;

                if (Math.Abs(farDistance.y) <= maxDistance * 1.1 && !Physics.CheckBox(testPosFarSideOfHit, testSize, rotation, mask))
                {
                    pos = getFurthestPoint(direction, pos, testPosFarSideOfHit);
                    continue; 
                }
                
                if (!Physics.CheckBox(testPosCloseToShip, testSize, rotation, mask))
                {
                    pos = getFurthestPoint(direction, pos, testPosCloseToShip);
                    continue;
                }
            }

            return pos;
        }

        private Vector3 getFurthestPoint(Vector3 direction, Vector3 a, Vector3 b)
        {
            if (direction == Vector3.up)
            {
                return (a.y > b.y) ? a : b;
            }
            
            return (a.y < b.y) ? a : b;
        }

        private Vector3 GetPosition(bool isAbove = false)
        {
            if (isAbove)
            {
                return transform.position + Vector3.up * _maxVerticalDistance;
            }

            return transform.position + Vector3.down * _maxVerticalDistance;
        }
        
        //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            return;
#endif
            if (!_collider)
                return;
            Gizmos.color = Color.green;

            var targetPositionBelow = GetTargetPosition(_collider.bounds.center, _size, Vector3.down, transform.localRotation, _maxVerticalDistance, _mask);
            var targetPositionAbove = GetTargetPosition(_collider.bounds.center, _size, Vector3.up, transform.localRotation, _maxVerticalDistance, _mask);
            Gizmos.DrawWireCube(targetPositionBelow, _size);
            Gizmos.DrawWireCube(targetPositionAbove, _size);
        }

        public void DoAbilityPrimary()
        {
            AttemptToTeleport();
        }
        
        public void DoAbilitySecondary()
        {
            AttemptToTeleport();
        }
        
        
        public void ExitAbilityPrimary()
        {
            
        }


        public void ExitAbilitySecondary()
        {

        }


        public void AttemptToTeleport()
        {
            float verticalDistance = GameManager.Instance.playerController.GetAxis("MoveVertical");
            float horizontalDistance = GameManager.Instance.playerController.GetAxis("MoveHorizontal");
            
            if (!GameManager.Instance.playerShip.teleportActive 
                && (verticalDistance > 0.3f || verticalDistance < -0.3f))
            {
                GameManager.Instance.playerShip.teleportActive = true;
                
                if (verticalDistance > 0.3f)
                {
                    DoTeleport(TeleportDirectionEnum.Up);
                } else
                {
                    DoTeleport(TeleportDirectionEnum.Down);
                }
            }
            else if (!GameManager.Instance.playerShip.teleportActive
                     && horizontalDistance > 0.3f)
            {
                GameManager.Instance.playerShip.teleportActive = true;
                DoTeleport(TeleportDirectionEnum.Forward);
            }
        }
        
        
        public void DoTeleport(TeleportDirectionEnum direction)
        {   
            if (playerLifeSystem.CurrentHP < BatteryChargeRequired || isLockedOut == true)
            {               
                _playerShip.teleportActive = false;
                return;
            }


            var maxDistance = Distance;

            if (direction == TeleportDirectionEnum.Down)
            {
                /*if (_collider.bounds.center.y - maxDistance < MinScreenHeight)
                {
                    maxDistance = _collider.bounds.center.y - MinScreenHeight;
                }*/
                _targetVector = GetTargetPosition(_collider.bounds.center, _size, Vector3.down, transform.localRotation,  maxDistance, _mask);
                _teleportSpeed = 0.02f;
            }
            else if (direction == TeleportDirectionEnum.Up)
            {
                /*if (maxDistance + _collider.bounds.center.y > MaxScreenHeight)
                {
                    maxDistance = MaxScreenHeight - _collider.bounds.center.y;
                }*/
                _targetVector = GetTargetPosition(_collider.bounds.center, _size, Vector3.up, transform.localRotation,  maxDistance, _mask);
                _teleportSpeed = 0.02f;
            }
            else
            {
                if (!_hasForwardTeleport)
                {
                    return;
                }
                
                
                maxDistance *= 1.75f;
                
                _targetVector = GetTargetPosition(_collider.bounds.center, _size, Vector3.right,
                    transform.localRotation, maxDistance, _mask);
                _teleportSpeed = 0.33f;
            }

            _collider.enabled = false;
            CoolDownTime = cooldownPenalty;
            PlayerLifeSystem.SpendLife(BatteryChargeRequired);
            _activationsIndex++;
            StartCoroutine(_playerShip.TFX.TeleportAction());
            //transform.position = _targetVector;
            transform.DOMove(_targetVector, 0).SetEase(Ease.OutCirc).OnComplete(TeleportMoveComplete);
            
        }

        //Built this method to allow for delay to give the FX time to run
        public void TeleportMoveComplete()
        {
            StartCoroutine(_playerShip.TFX.TeleportComplete());
            StartCoroutine(ExitTeleport());
            UsedRightAbility?.Invoke();
        }

        private IEnumerator ExitTeleport()
        {
            while (_playerShip.TFX.teleportFXEnd == false)
            {
                yield return null;
            }
            _collider.enabled = true;
            
            StartCoroutine(TeleportCooldown());
        }
        
        public IEnumerator TeleportCooldown()
        {
            _playerShip.teleportActive = false;

            if (_activationsIndex == _lockoutThreshold)
            {
                isLockedOut = true;
                CoolDownTime = lockoutPenalty;
                eBar.valueCurrent = 100;
            }
            
            DOTween.To(()=> eBar.valueCurrent, x=> eBar.valueCurrent = x, 0, CoolDownTime);
            
            while (CoolDownTime > 0.0f)
            {
                CoolDownTime -= Timekeeper.instance.Clock("Player").fixedDeltaTime;
                yield return null;
            }
            
            eBar.valueCurrent = 0;
            
            isLockedOut = false;
            _activationsIndex = 0;
            CoolDownTime = 0;

            yield return null;
        }
        
    }
}