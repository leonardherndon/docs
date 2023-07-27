using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class Vertical : MonoBehaviour, IUpgrade<float>
    {
        private GearMoveApp _gearMoveApp;
        [ShowInInspector] private float _modifier;
        [ShowInInspector] private float _increaseValue;
        [ShowInInspector] private float _decreaseValue;
        [ShowInInspector] private float _original;

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
            _original = _gearMoveApp.BaseVerticalSpeed;
        }

        public void OnDestroy()
        {
            ResetModifier();
        }

        private void ApplySpeedModifier()
        {
            Debug.LogFormat("Applying the modifer: {0}", _modifier);
            _gearMoveApp.BaseVerticalSpeed *= _modifier;
        }

        private void ResetModifier()
        {
            if (Math.Abs(_modifier) < 0.00001)
            {
                return;
            }

            Debug.LogFormat("Resetting the modifier.");
            if (Math.Abs(_modifier - 1.0) < 0.00001)
            {
                Debug.LogFormat("Going to hard set the value back to the original value: {0}", _original);
                _gearMoveApp.BaseVerticalSpeed = _original;
                return;
            }

            Debug.LogFormat("Base Vertical Speed was {0}. Now dividing by the modifier {1}",
                _gearMoveApp.BaseVerticalSpeed, _modifier);

            _gearMoveApp.BaseVerticalSpeed /= _modifier;
        }
        public void Remove()
        {
            Destroy(this);
        }
    }
}