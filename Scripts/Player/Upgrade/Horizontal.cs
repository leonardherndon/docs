using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class Horizontal: MonoBehaviour, IUpgrade<float>
    {
        private GearMoveApp _gearMoveApp;
        [ShowInInspector]
        private float _modifier;
        [ShowInInspector]
        private float _increaseValue;
        [ShowInInspector]
        private float _decreaseValue;
        [ShowInInspector]
        private float _original;
        
        public float Modifier
        {
            get => (float) (_modifier - 1.0);
            set
            {
                ResetModifier();
                _modifier = (float) 1.0 + value; 
                ApplySpeedModifier();
            }
        }

        public void Awake()
        {
            _gearMoveApp = GetComponent<GearMoveApp>();
            _original = _gearMoveApp.BaseHorizontalSpeed;
        }

        public void OnDestroy()
        {
            ResetModifier();
        }

        private void ApplySpeedModifier()
        {
            if (_gearMoveApp == null)
            {
                return;
            }
            
            _gearMoveApp.BaseHorizontalSpeed *= _modifier;
        }
        
        private void ResetModifier()
        {
            if (Math.Abs(_modifier) < 0.00001)
            {
                return;
            }
            
            if (Math.Abs(_modifier - 1.0) < 0.00001)
            {
                _gearMoveApp.BaseHorizontalSpeed = _original;
                return;
            }
            
            _gearMoveApp.BaseHorizontalSpeed /= _modifier;
        }
        public void Remove()
        {
            Destroy(this);
        }
    }
}