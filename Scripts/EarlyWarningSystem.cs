using System.Collections.Generic;
using ChromaShift.Scripts.Player;
using ChromaShift.Scripts.Player.Upgrade;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChromaShift.Scripts
{
    public class EarlyWarningSystem : MonoBehaviour, IEarlyWarningSystem, IUpgrade
    {
        private PlayerShip _playerShip;
        private Collider[] _colliders;
        private Collider[] _filteredColliders;
        private Camera _camera;
        private GameObject _primitive;
        private List<GameObject> _primitives;
        [ShowInInspector] private float _modifier;
        private string _hardColorPassIcon = "";
        private string _softColorPassIcon = "";
        private Material _hardColorPass;
        private Material _softColorPass;

        public float Modifier
        {
            get => _modifier;
            set => _modifier = value;
        }

        public void OnDestroy() {}

        public void Remove() {}

        public void Awake()
        {
            _playerShip = GetComponent<PlayerShip>();
            _colliders = new Collider[200];
            _camera = GameObject.FindObjectOfType<Camera>();
            _primitives = new List<GameObject>();

            if (_hardColorPassIcon != "")
            {
                var hardTexture = Resources.Load(_hardColorPassIcon) as Texture2D;
                _hardColorPass = new Material(Shader.Find("Diffuse"));
                
                _hardColorPass.mainTexture = hardTexture;
            }
            
            if (_softColorPassIcon != "")
            {
                var softTexture = Resources.Load(_softColorPassIcon) as Texture2D;
                _softColorPass = new Material(Shader.Find("Diffuse"));
                
                _softColorPass.mainTexture = softTexture;
            }
        }

        public void FixedUpdate()
        {
            var layerMask = 1 << 11;

            layerMask = ~layerMask;

            var size = Physics.OverlapSphereNonAlloc(
                _playerShip.transform.position,
                _modifier,
                _colliders,
                layerMask
            );

            _filteredColliders = Filter(_colliders, size);

            SetPrimitives(_filteredColliders);
        }

        private void Update()
        {
            DisplayIndicator();
        }

        private void SetPrimitives(Collider[] colliders)
        {
            if (_primitives.Count == colliders.Length)
            {
                return;
            }

            if (_primitives.Count < colliders.Length)
            {
                var delta = colliders.Length - _primitives.Count;
                for (var i = 0; i < delta; i++)
                {
                    var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    var pos = NormalizeScreenPosition(_camera, colliders[i].transform.position);
                    var rot = Quaternion.Euler(-90, 0, 0);

                    var collisionObjectController = colliders[i].GetComponent<CollisionController>();
                    var earlyWarnableComponent = colliders[i].GetComponent<EarlyWarnableComponent>();

                    // @TODO Use earlyWarnableComponent.IsSoftColorPass for now to determine which icon needs to be displayed

                    var color = ColorManager.Instance.ConvertEnumToColor(collisionObjectController.CSM.CurrentColor);
                    var mat = plane.GetComponent<Renderer>().material;

                    plane.transform.position = _camera.ScreenToWorldPoint(pos);
                    plane.transform.rotation = rot;
                    plane.transform.parent = _camera.transform;

                    if (earlyWarnableComponent.IsSoftColorPass && _softColorPassIcon != "")
                    {
                        mat = _softColorPass;
                    } else if (!earlyWarnableComponent.IsSoftColorPass && _hardColorPassIcon != "")
                    {
                        mat = _hardColorPass;
                    }
                    
                    mat.color = color;

                    _primitives.Add(plane);
                }
            }
            else if (_primitives.Count > colliders.Length)
            {
                var delta = _primitives.Count - colliders.Length;

                for (var i = 0; i < delta; i++)
                {
                    Destroy(_primitives[i]);
                    _primitives.RemoveAt(i);
                }
            }

            DisplayIndicator();
        }

        private void LateUpdate()
        {
            if (_filteredColliders == null)
            {
                return;
            }

            if (_filteredColliders.Length <= 0)
            {
                return;
            }

            DisplayIndicator();
        }

        public void DisplayIndicator()
        {
            if (_filteredColliders.Length <= 0)
            {
                return;
            }

            for (var i = 0; i < _filteredColliders.Length; i++)
            {
                var currentPos = _filteredColliders[i].transform.position;
                var generalPos = currentPos - _filteredColliders[i].bounds.size;

                var screenPos = NormalizeScreenPosition(_camera, generalPos);

                var worldPos = _camera.ScreenToWorldPoint(screenPos);

                if (IsOnScreen(_camera, worldPos) || IsToTheLeft(_camera, worldPos))
                {
                    SetOffscreen(_primitives[i]);
                    continue;
                }

                _primitives[i].transform.position = worldPos;
            }
        }

        private bool IsToTheLeft(Camera camera, Vector3 worldPos)
        {
            return (camera.transform.position.x > worldPos.x);
        }

        private void SetOffscreen(GameObject gameObject)
        {
            gameObject.transform.position = _camera.ScreenToWorldPoint(new Vector3(-10, -10));
        }

        private bool IsOnScreen(Camera camera, Vector3 worldPosition)
        {
            var screenPos = camera.WorldToScreenPoint(worldPosition);
            bool isWithinWidth = (screenPos.x < camera.scaledPixelWidth && screenPos.x >= 0);
            bool isWithinHeight = (screenPos.y < camera.scaledPixelHeight && screenPos.y >= 0);

            return isWithinHeight && isWithinWidth;
        }

        /// <summary>
        /// Need to take in the camera's screen size, and the targeted screen position of the indicator and then normalize is to within the constraints of the screen.
        /// </summary>
        private Vector3 NormalizeScreenPosition(Camera camera, Vector3 worldPosition)
        {
            var screenSpace = camera.WorldToScreenPoint(worldPosition);
            var width = camera.scaledPixelWidth;
            var height = camera.scaledPixelHeight;

            if (screenSpace.x < 0)
            {
                screenSpace.x = 0;
            }
            else if (screenSpace.x > width)
            {
                screenSpace.x = width;
            }

            if (screenSpace.y < 0)
            {
                screenSpace.y = 0;
            }
            else if (screenSpace.y > height)
            {
                screenSpace.y = height;
            }

            return screenSpace;
        }

        private Collider[] Filter(Collider[] colliders, int size)
        {
            var filtered = new System.Collections.Generic.List<Collider>();

            for (int i = 0; i < size; i++)
            {
                var ew = colliders[i].GetComponent<EarlyWarnableComponent>();

                if (ew == null)
                {
                    continue;
                }

                if (!ew.IsEarlyWarnable)
                {
                    continue;
                }

                filtered.Add(colliders[i]);
            }

            return filtered.ToArray();
        }
    }
}