using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChromaShift.Scripts
{
    public class EarlyWarnableComponent : MonoBehaviour
    {
        private EarlyWarnable _earlyWarnable;
        [SerializeField, HideInInspector] private bool _isEarlyWarnable = true;

        [SerializeField, HideInInspector] private bool _isSoftColorPass;
        // could add in a left right flag here and use that.

        [ShowInInspector]
        public bool IsEarlyWarnable
        {
            get => _isEarlyWarnable;
            set
            {
                _isEarlyWarnable = value;
                CreateEarlyWarnable(value);
            }
        }

        [ShowInInspector]
        public bool IsSoftColorPass
        {
            get => _isSoftColorPass; 
            set => _isSoftColorPass = value;
        }

        private void Awake()
        {
            CreateEarlyWarnable(_isEarlyWarnable);
        }

        private void CreateEarlyWarnable(bool isWarnable)
        {
            _earlyWarnable = new EarlyWarnable(isWarnable);
        }
    }
}