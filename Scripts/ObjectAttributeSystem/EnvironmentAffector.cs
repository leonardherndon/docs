using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ChromaShift.Scripts.ObjectAttributeSystem
{
    
    public class EnvironmentAffector : MonoBehaviour
    {
        //TODO I NEED TO GET THE ICONS TO RENDER FOR THE ATTRIBUTES
        
        [SerializeField] public AttributeType attrType;
        private EnviornmentAffectorAttributeManager _enviornmentAffectorAttributeManager;
        [SerializeField] private float _currentValue;
        public float CurrentValue
        {
            get => _currentValue;
            set => _currentValue = value;
        }

        public GameObject effectGroup;
        [SerializeField] public UnityEngine.UIElements.Image icon;
        [SerializeField] public Image iconUI;
        [SerializeField] public EnergyBar barUI;
        public GameObject affectorGroupPrefab;
        [SerializeField] private List<EnvironmentAffectorAttributeThreshold> _thresholds;

        public void Start()
        {
            _enviornmentAffectorAttributeManager = gameObject.GetComponentInParent<EnviornmentAffectorAttributeManager>();
        }
        public void CheckThreshold()
        {
            DisplayCurrentValue();
            foreach (EnvironmentAffectorAttributeThreshold threshold in _thresholds)
            {
                if(!threshold.isActive)
                    continue;
                if (_currentValue >= threshold.triggerNumber)
                {
                    threshold.DoTriggerAction();
                    threshold.isActive = false;
                }
                else
                {
                    threshold.isActive = true;
                }
            }
        }

        [PropertyOrder (0)]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 0.3f)]
        public void CreateNewThreshold()
        {
            var newThreshold = gameObject.AddComponent<EnvironmentAffectorAttributeThreshold>();
            _thresholds.Add(newThreshold);
        }

        public void DisplayCurrentValue()
        {
            if (effectGroup == null)
            {
                CreateEffectUIGroup();
            }

            barUI.valueCurrent = (int)_currentValue;
        }

        public void CreateEffectUIGroup()
        {
            effectGroup = Instantiate(affectorGroupPrefab, Vector3.zero, Quaternion.identity,
                GameObject.Find("StatusDropbox").transform);
            //iconUI = effectGroup.GetComponentInChildren<Image>();
            //iconUI.sprite = icon.image;
        }
    }
   
}