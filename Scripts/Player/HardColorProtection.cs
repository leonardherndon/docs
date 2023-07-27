using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class HardColorProtection: MonoBehaviour, IUpgrade<float>
    {
        private PlayerShip _playerShip;
        public bool hasHardColorPassProtection;
        public float Modifier { get; set; }
        public void Awake()
        {
            _playerShip = GetComponent<PlayerShip>();
            hasHardColorPassProtection = true;
        }

        public void OnDestroy()
        {
            hasHardColorPassProtection = false;
        }
        public void Remove()
        {
            Destroy(this);
        }
    }
}