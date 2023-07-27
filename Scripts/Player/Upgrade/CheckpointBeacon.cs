using Sirenix.OdinInspector;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class CheckpointBeacon : MonoBehaviour, IUpgrade<bool>
    {
        private bool _modifier;
        public bool Modifier
        {
            get => _modifier;
            set => _modifier = value;
        }

        public void Awake()
        {
            // throw new System.NotImplementedException();
        }

        public void OnDestroy()
        {
            // throw new System.NotImplementedException();
        }

        public void Remove()
        {
            // throw new System.NotImplementedException();
        }
    }
}