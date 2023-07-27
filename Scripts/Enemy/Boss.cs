using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ChromaShift.Scripts.Enemy.Resource;
using Chronos;
using DG.Tweening;
using QuickEngine.Extensions;
using Rewired.UI.ControlMapper;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ChromaShift.Scripts.Enemy
{
    /// <summary>
    ///     Boss is the overall collective functionality of the boss. From spawning prefabs to handling hits on itself.
    /// </summary>
    public class Boss : MonoBehaviour
    {
        [SerializeField] public float _onHitTimeDelay = .5f;
        [SerializeField] public float _telegraphColorTime = .3f;
        [SerializeField] private float _invincibleTime = 5f;
        [SerializeField] private float _fadeToGray = 1f;
        [SerializeField] private bool _isInvincible = false;
        [SerializeField] private int _health = 5;
        
        public float DistanceFromPlayer = 60.0f;
        
        [ShowInInspector] private Queue<IResourceSet> _queue;
        #if UNITY_EDITOR
        [HorizontalGroup("queue00", LabelWidth = .5f),ShowInInspector] private Queue<IResourceSet> _queue00;
        [HorizontalGroup("queue00"), Button(ButtonSizes.Medium)]
        public void loadQueue00()
        {
            LoadQueue(ref _queue00);
        }
        
        [HorizontalGroup("queue01", LabelWidth = .5f),ShowInInspector] private Queue<IResourceSet> _queue01;
        [HorizontalGroup("queue01"), Button(ButtonSizes.Medium)]
        public void loadQueue01()
        {
            LoadQueue(ref _queue01);
        }

        [HorizontalGroup("queue02", LabelWidth = .5f),ShowInInspector] private Queue<IResourceSet> _queue02;
        [HorizontalGroup("queue02"), Button(ButtonSizes.Medium)]
        public void loadQueue02()
        {
            LoadQueue(ref _queue02);
        }

        [HorizontalGroup("queue03", LabelWidth = .5f),ShowInInspector] private Queue<IResourceSet> _queue03;
        [HorizontalGroup("queue03"), Button(ButtonSizes.Medium)]
        public void loadQueue03()
        {
            LoadQueue(ref _queue03);
        }
        #endif
        [ShowInInspector] private MovementStack _move00;
        [ShowInInspector] private MovementStack _move01;
        [ShowInInspector] private MovementStack _move02;
        [ShowInInspector] private MovementStack _move03;

        private Animator _asheAnimator;
        private SkinnedMeshRenderer _body;
        private GameObject _bodyLaser;
        private GetResourceSetQueue _currentGetResourceSetQueue;
        private bool _hasTelegraphedColor;
        private bool _hasTimeStarted;
        private PlayerShip _playerShip;
        private SkinnedMeshRenderer _port;
        private GameObject _portLaser;
        private GameObject _portSpawn;
        private SkinnedMeshRenderer _starboard;
        private GameObject _starboardLaser;
        private GameObject _starboardSpawn;
        private float _timeTracker;
        private Clock _clock;
        
        public delegate void BossHealthChange(int currentHealth);
        public event BossHealthChange HealthChangeEvent;
        // @TODO Implement this event and add in some properties for this class to hold some movement stacks to call the event with.
        private event MovableObjectController.HandleNewMovementStack MovementSetEvent;

        public delegate void LoadResourceSet(string path);

        public event LoadResourceSet NewResourceEvent;

        private void Start()
        {
            _playerShip = GameManager.Instance.playerShip;
            _bodyLaser = GameObject.Find("BodyLaser");
            _starboardSpawn = GameObject.Find("StarboardSpawn");
            _portSpawn = GameObject.Find("PortSpawn");
            _portLaser = GameObject.Find("PortLaser");
            _starboardLaser = GameObject.Find("StarboardLaser");
            _asheAnimator = GetAsheAnimator();
            _hasTimeStarted = false;
            _hasTelegraphedColor = false;
            _clock = Timekeeper.instance.Clock("Enemy");

//            _currentGetResourceSetQueue = StartQueue;

//            _queue = _currentGetResourceSetQueue();

            this.NewResourceEvent += this.LoadResourceSetFromPath;

            var body = GameObject.Find("Drone_Ashe_Boss");
            var port = GameObject.Find("Drone_Ashe_Boss/Left_Arm");
            var starboard = GameObject.Find("Drone_Ashe_Boss/Right_Arm");

            if (!body)
                throw new InvalidOperationException(
                    "The body GameObject, Drone_Ashe_Boss, was not found. This is needed.");
            _body = body.GetComponent<SkinnedMeshRenderer>();

            if (!port)
                throw new InvalidOperationException(
                    "The port GameObject, Drone_Ashe_Boss/Left_Arm, was not found. This is needed.");
            _port = port.GetComponent<SkinnedMeshRenderer>();

            if (!starboard)
                throw new InvalidOperationException(
                    "The starboard GameObject, Drone_Ashe_Boss/Right_Arm, was not found. This is needed.");
            _starboard = starboard.GetComponent<SkinnedMeshRenderer>();

            if (_body.material.HasProperty("_EmissionColor"))
                _body.material.DOColor(ColorManager.Instance.colorArray[(int) GameColor.Grey], "_EmissionColor", .75f);

            if (_port.material.HasProperty("_EmissionColor"))
                _port.material.DOColor(ColorManager.Instance.colorArray[(int) GameColor.Grey], "_EmissionColor", .75f);

            if (_starboard.material.HasProperty("_EmissionColor"))
                _starboard.material.DOColor(ColorManager.Instance.colorArray[(int) GameColor.Grey], "_EmissionColor",
                    .75f);
        }

        /// <summary>
        ///     HandleLaserHits takes an array of hits to process for collision with the Boss.
        /// </summary>
        /// <param name="hits"></param>
        public void HandleLaserHit(RaycastHit hit)
        {
            
                if (hit.transform.name != transform.name)
                {
                    return;
                }

                _health--;
                PlayShakeAnimation();
                DelayNextDrop();
                HealthChangeEvent(_health);
                SetInvincible();
        }

        /// <summary>
        ///     GetAsheAnimator finds and gets the animator needed.
        /// </summary>
        /// <returns></returns>
        private Animator GetAsheAnimator()
        {
            var ashe = GameObject.Find("Drone_Ashe_Boss");
            if (!ashe)
                throw new Exception(
                    "No GameObject found. Need to have a GameObject call Drone_Ashe_Boss for the boss ship.");

            return ashe.GetComponent<Animator>();
        }

        /// <summary>
        ///     StartQueue build the initial queue to use cycle through.
        /// </summary>
        /// <returns>A queue of IResourceSet to be processed.</returns>
        private Queue<IResourceSet> StartQueue()
        {
            var q = new Queue<IResourceSet>();

            
//            q.Enqueue(new ResourceSet(5f, new BossLaserLarge(GameColor.White), new  BossLaserSmall(GameColor.Blue),
//                new Null(), new BossLaserSmall(GameColor.Red), new Null()));
            q.Enqueue(new ResourceSet(5f, new Null(), new Null(), new Rocket(), new Null(), new Null()));

            return q;
        }

        /// <summary>
        ///     OnCollisionEnter handles the logic on what collisions it cares about and what to do.
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Obstacle")) return;

            _health--;
            DelayNextDrop();
            PlayShakeAnimation();
            var otherHostileObject = other.collider.GetComponent<Hostile>();

            //if (otherHostileObject) otherHostileObject.KillObject(false);
        }

        /// <summary>
        ///     PlayShakeAnimation plays the shake animation.
        /// </summary>
        private void PlayShakeAnimation()
        {
            if (_asheAnimator == null)
            {
                Debug.LogError("Missing animator for the Ashe Boss Ship");
                return;
            }

            _asheAnimator.Play("ashe_hit");
        }

        /// <summary>
        ///     DelayNextDrop increase the time until the next set of boss abilities occur.
        /// </summary>
        private void DelayNextDrop()
        {
            if (_queue.Count <= 0) return;

            _queue.Peek().IncreaseDeployTime(_onHitTimeDelay);
        }

        /// <summary>
        ///     ManageQueue handles looping through the ResourceSet to telegraph the abilities, deploy the resources.
        /// </summary>
        private void ManageQueue()
        {
            if (_queue == null || _queue.Count <= 0) return;
//            if (_queue.Count <= 0) _queue = _currentGetResourceSetQueue();

            if (!_hasTelegraphedColor && _timeTracker > _queue.Peek().GetDeployTime() - 1f)
            {
                TelegraphColor(_queue.Peek());
                _hasTelegraphedColor = true;
            }

            if (_timeTracker < _queue.Peek().GetDeployTime()) return;

            // @TODO Need a better place/way to do this.
            _hasTelegraphedColor = false;
            _timeTracker = 0f;

            Debug.Log("Time to work the current Resource Set.");
            var rs = _queue.Dequeue();

            DeployResources(rs);
        }

        /// <summary>
        ///     TelegraphColor updates the materials needed to show the color for an ability that is about to happen.
        /// </summary>
        /// <param name="resourceSet">The recourse set to process.</param>
        private void TelegraphColor(IResourceSet resourceSet)
        {
            var body = resourceSet.GetBodyResource();
            var portArm = resourceSet.GetPortArmResource();
            var starboardArm = resourceSet.GetStarboardArmResource();

            UpdateColor(body, _body.material);
            UpdateColor(portArm, _port.material);
            UpdateColor(starboardArm, _starboard.material);
        }

        /// <summary>
        ///     UpdateColor is to telegraph the color of the boss's ability about to happen from a particular part.
        /// </summary>
        /// <param name="resource">The resource to process</param>
        /// <param name="material">The material to update</param>
        private void UpdateColor(IResource resource, Material material)
        {
            material.DOColor(ColorManager.Instance.ConvertEnumToColor(resource.GetColor()), "_EmissionColor",
                _telegraphColorTime);
        }

        /// <summary>
        ///     DeployResources deploys get of the different resources; Body, Port Arm, Port Trap, Starboard Arm, Starboard Trap.
        /// </summary>
        /// <param name="resourceSet">The collection of resources to deploy</param>
        private void DeployResources(IResourceSet resourceSet)
        {
            var body = resourceSet.GetBodyResource();
            DeployResource(body, _bodyLaser.transform.position, _body.material);

            var portArm = resourceSet.GetPortArmResource();
            DeployResource(portArm, _portLaser.transform.position, _port.material);

            var portTrap = resourceSet.GetPortFlapResource();
            DeployResource(portTrap, _portSpawn.transform.position, null);

            var starboardArm = resourceSet.GetStarboardArmResource();
            DeployResource(starboardArm, _starboardLaser.transform.position, _starboard.material);

            var starboardTrap = resourceSet.GetStarboardFlapResource();
            DeployResource(starboardTrap, _starboardSpawn.transform.position, null);
        }

        /// <summary>
        ///     DeployResource will take the given resource and spawn the prefab at the given location. The material gets set back
        ///     to gray.
        /// </summary>
        /// <param name="resource">The Resource to spawn.</param>
        /// <param name="spawnPoint">The location within the world coordinates to space the prefab.</param>
        /// <param name="material">The targeted Material to set back to gray.</param>
        private void DeployResource(IResource resource, Vector3 spawnPoint, Material material)
        {
            if (material)
                material.DOColor(ColorManager.Instance.colorArray[(int) GameColor.Grey], "_EmissionColor", _fadeToGray);

            if (resource.GetPrefabPath().IsNullOrEmpty()) return;

            var lt = Instantiate(
                Resources.Load(resource.GetPrefabPath(), typeof(GameObject)),
                spawnPoint,
                Quaternion.identity,
                transform
            ) as GameObject;
            
            Debug.Log("Weapon Deployed: " + lt.gameObject.name);
            if (!resource.IsAttached()) lt.transform.parent = null;

            LineRenderer lineRenderer = lt.GetComponent<LineRenderer>();
            if (lineRenderer)
            {
                var c =  ColorManager.Instance.GetColor(resource.GetColor());

                Shader s = Shader.Find("Standard");

                Material m = new Material(s);

                if (!m.IsKeywordEnabled("_EMISSION"))
                {
                    m.EnableKeyword("_EMISSION");
                }

                if (m.HasProperty("_EmissionColor"))
                {
                    m.SetColor("_EmissionColor", c);
                    m.EnableKeyword("_EmissionColor");
                }

                if (m.HasProperty("_Color"))
                {
                    m.SetColor("_Color", c);
                    m.EnableKeyword("_Color");
                }

                lineRenderer.material = m;
            }

            CollisionController CoC = lt.GetComponent<CollisionController>();
            
            if (CoC)
            {
                var cIndex = resource.GetColor();

                CoC.CSM.ChromaShift(cIndex);
            }
        }

        private void Update()
        {
            if (_health <= 0) Debug.Log("Kill the boss!");

//            var delta = transform.position.x - _playerShip.transform.position.x;

//            if (delta < DistanceFromPlayer) StayInFrontOfPlayer(DistanceFromPlayer);

            if (!_hasTimeStarted && _body.isVisible) _hasTimeStarted = true;

            if (!_hasTimeStarted) return;

            _timeTracker += _clock.deltaTime;

            ManageQueue();
        }

        /// <summary>
        ///     StayInFrontOfPlayer is a basic hack to keep the boss ship N units in front of the player ship.
        /// </summary>
        /// <param name="distance"></param>
        public void StayInFrontOfPlayer(float distance)
        {
            var position = gameObject.transform.position;
            var targetPos = new Vector3(
                _playerShip.transform.position.x + distance,
                position.y,
                position.z
            );

            gameObject.transform.position = targetPos;
        }

        private delegate Queue<IResourceSet> GetResourceSetQueue();

        public void SetInvincible()
        {
            _isInvincible = true;
            StartCoroutine(SetToBeVulnerable());
        }

        public IEnumerator SetToBeVulnerable()
        {
            yield return new WaitForSeconds(_invincibleTime);
            _isInvincible = false;

            yield return null;
        }
        #if UNITY_EDITOR
        private void LoadQueue(ref Queue<IResourceSet> queue)
        {
            var path = EditorUtility.OpenFilePanel("Select Resource set", "", "rs");
            var bf = new BinaryFormatter();
            var fs = File.Open(path, FileMode.Open);

            var content = bf.Deserialize(fs);
            fs.Close();

            var rc = (ResourceCollection) content;

            Queue<IResourceSet> tmpQueue = new Queue<IResourceSet>();

            foreach (var resourceSet in rc.data)
            {
                tmpQueue.Enqueue(resourceSet);
            }
            queue = tmpQueue;
        }
        #endif


        public void LoadResourceSetFromPath(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            Debug.LogFormat("Want to load in resource {0}", filename);
                
            ResourceCollection resourceCollection = Resources.Load<ResourceCollection>(Path.Combine("Assets", "ChromaShift","Resources","BossArsenal","Scripts", filename));
            
            //var resourceCollection = AssetDatabase.LoadAssetAtPath<ResourceCollection>(Path.Combine("Assets", filename));
            
            Debug.LogFormat("The resource collection loaded: {0}", resourceCollection);

            Queue<IResourceSet> queue = new Queue<IResourceSet>();

            foreach (var resourceSet in resourceCollection.data)
            {
                queue.Enqueue(resourceSet);
            }

            _queue = queue;

            return;
        }
    
    }
}