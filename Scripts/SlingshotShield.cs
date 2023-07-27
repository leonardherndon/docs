using System;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using UnityEngine;

namespace ChromaShift.Scripts
{
    public interface ISlingshotShield
    {
        GameObject Target { set; get; }

        /// <summary>
        /// The implementation to move the current object towards the target
        /// </summary>
        IEnumerator MoveToTarget();

        /// <summary>
        /// How long until the this should start moving.
        /// </summary>
        /// <param name="time"></param>
        void Delay(float time);
    }

    public class SlingshotShield : MonoBehaviour, ISlingshotShield
    {
        private float _timeDelay;
        private PlayerShip _playerShip;
        private Vector3 _originalPosition;
        [SerializeField] private float _abilityCost = 15f; 
        [SerializeField] private float _totalMovementTime = 1f;
        [SerializeField] private float _speed = 2f;
        private Vector3 _currentVelocity = Vector3.zero;
        private Rigidbody _rigidbody;
        [SerializeField] private float _maxDistance = 2.5f;
        private Rewired.Player _iRewirePlayer;

        public GameObject Target { get; set; }
        public float TotalMovementTime { get; set; }

        public IEnumerator MoveToTarget()
        {
            transform.localPosition = _playerShip.transform.localPosition;
            _originalPosition = transform.localPosition;

            yield return new WaitForSeconds(_timeDelay);
            
            // The string "ActiveAbilityLeft" is also being used in the script InputSystem.cs
            yield return new WaitWhile(() => _iRewirePlayer.GetButton("ActiveAbilityLeft"));

            float currentMovementTime = 0f;

            while (isAwayFromPlayerShip())
            {
                currentMovementTime += Time.deltaTime;

                moveTowardPlayerShip(currentMovementTime);
                yield return null;
            }

            Destroy(gameObject);
        }

        public void SetPlayerShip(ref PlayerShip playerShip)
        {
            _playerShip = playerShip;
        }

        private void Start()
        {
            GetComponent<ChromaShiftManager>().ChromaShift(_playerShip.CSM.CurrentColor);
            GetComponent<LifeSystemController>().SpendLife(_abilityCost);
            StartCoroutine(MoveToTarget());
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _iRewirePlayer = GameManager.Instance.playerController;
        }

        private void Update()
        {
        }

        public void Delay(float time)
        {
            _timeDelay = time;
        }

        private bool isAwayFromPlayerShip()
        {
            var d = Vector3.Distance(transform.localPosition, _playerShip.transform.localPosition);

            return d >= 1f;
        }

        private void moveTowardPlayerShip(float currentMovementTime)
        {
            var md = (currentMovementTime / _totalMovementTime) * _maxDistance;

            if (md > _maxDistance)
            {
                md = _maxDistance;
            }

            var v = Vector3.MoveTowards(transform.position, _playerShip.transform.position, md);

            var p = _rigidbody.position + v;
            
            _rigidbody.MovePosition(v);
        }
    }
}