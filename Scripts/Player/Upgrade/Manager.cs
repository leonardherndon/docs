using Sirenix.OdinInspector;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class Manager : MonoBehaviour, IManager
    {
        private PlayerShip _playerShip;
        private IFactory _factory;
        [SerializeField, HideInInspector] private UpgradePreferences _upgradePreferences;

        [SerializeField, HideInInspector] private float _coolDownModifier;
        [SerializeField, HideInInspector] private float _horizontalModifier;
        [SerializeField, HideInInspector] private float _verticalModifier;
        [SerializeField, HideInInspector] private float _teleportModifier;
        [SerializeField, HideInInspector] private float _earlyWarningDistance;
        [SerializeField, HideInInspector] private bool _hasCheckpointBeacon;

        [ShowInInspector]
        public bool HasCoolDownAbility
        {
            get => _upgradePreferences.HasCoolDown;
            set
            {
                _upgradePreferences.HasCoolDown = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
            }
        }
        
        [ShowInInspector]
        public bool HasForwardTeleport
        {
            get => _upgradePreferences.HasForwardTeleport;
            set
            {
                _upgradePreferences.HasForwardTeleport = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
            }
        }

        [ShowInInspector, MinValue(-0.999), MaxValue(0.999)]
        public float CoolDownModifier
        {
            get { return _coolDownModifier; }
            set
            {
                _coolDownModifier = value;
                DoUpdate();
            }
        }

        [ShowInInspector]
        public bool HasHardColorProtection
        {
            get => _upgradePreferences.HasHardColor;
            set
            {
                _upgradePreferences.HasHardColor = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
            }
        }

        [ShowInInspector]
        public bool HasHorizontalAbility
        {
            get => _upgradePreferences.HasHorizontal;
            set
            {
                _upgradePreferences.HasHorizontal = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
            }
        }

        [ShowInInspector, MinValue(0.000), MaxValue(5.000)]
        public float HorizontalModifier
        {
            get { return _horizontalModifier; }
            set
            {
                _horizontalModifier = value;
                DoUpdate();
            }
        }

        [ShowInInspector]
        public bool HasNebulaProtection
        {
            get => _upgradePreferences.HasNebula;
            set
            {
                _upgradePreferences.HasNebula = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
            }
        }

        [ShowInInspector]
        public bool HasTeleportIncrease
        {
            get => _upgradePreferences.HasTeleportIncrease;
            set
            {
                _upgradePreferences.HasTeleportIncrease = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
            }
        }

        [ShowInInspector, MinValue(0.000), MaxValue(5.000)]
        public float TeleportModifier
        {
            get { return _teleportModifier; }
            set
            {
                _teleportModifier = value;
                DoUpdate();
            }
        }

        [ShowInInspector]
        public bool HasVerticalAbility
        {
            get => _upgradePreferences.HasVertical;
            set
            {
                _upgradePreferences.HasVertical = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
            }
        }

        [ShowInInspector, MinValue(0.000), MaxValue(5.000)]
        public float VerticalModifier
        {
            get => _verticalModifier;
            set
            {
                _verticalModifier = value;
                DoUpdate();
            }
        }

        [ShowInInspector]
        public bool HasCheckpointBeacon
        {
            get => _hasCheckpointBeacon;
            set
            {
                _hasCheckpointBeacon = value;
                _upgradePreferences.HasCheckpointBeacon = value;
                _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
                DoUpdate();
            }
        }

        public void Awake()
        {
            _playerShip = GetComponent<PlayerShip>();

            _factory = new Factory(_playerShip.gameObject, this, ref _upgradePreferences);

            _factory?.AttachUpgrades(_playerShip.gameObject, this, _upgradePreferences);
        }

        public PlayerShip GetPlayerShip()
        {
            return _playerShip;
        }

        public IFactory GetFactory()
        {
            return _factory;
        }

        private void DoUpdate()
        {
            _factory?.UpdateModifiers(this);
        }

        [ShowInInspector]
        public bool HasEarlyWarningSystem
        {
            get => _upgradePreferences.HasEarlyWarningSystem;
            set => _upgradePreferences.HasEarlyWarningSystem = value;
        }

        [ShowInInspector, MinValue(100f), MaxValue(500f)]
        public float EarlyWarningSystemDistance
        {
            get => _earlyWarningDistance;
            set => _earlyWarningDistance = value;
        }

        [ShowInInspector]
        public bool HasWhiteColorAbility
        {
            get => _upgradePreferences.HasWhiteColorAbility; 
            set => _upgradePreferences.HasWhiteColorAbility = value; 
            
        }
    }
}