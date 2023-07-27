using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class CoolDown: MonoBehaviour, IUpgrade<float>
    {
        private PlayerTeleportInterface _playerTeleport;
        private float _modifier;
        private float _shieldDelayTimer;
        private PlayerShip _playerShip;

        public float Modifier
        {
            get => (float) (_modifier + 1.0);
            set
            {
                ResetCoolDown();
                _modifier =  (float) 1.0 - value; 
                UpdateCoolDown();
            }

        }
        public void Awake()
        {
            _playerTeleport = GetComponent<PlayerTeleportInterface>();
            _playerShip = GetComponent<PlayerShip>();
        }

        public void OnDestroy()
        {
            ResetCoolDown();
        }

        private void UpdateCoolDown()
        {
            _playerTeleport.CoolDownTime *= _modifier;
            _playerShip.shieldFixedAbility.shieldDelayTime *= _modifier;
        }

        private void ResetCoolDown()
        {
            if (_modifier == 0)
            {
                return;
            }

            _playerTeleport.CoolDownTime /= _modifier;
            _playerShip.shieldFixedAbility.shieldDelayTime /= _modifier;
        }
        
        public void Remove()
        {
            Destroy(this);
        }
    }
}