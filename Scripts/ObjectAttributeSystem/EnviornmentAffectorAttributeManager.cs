using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.ObjectAttributeSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts.ObjectAttributeSystem
{
    
    public class EnviornmentAffectorAttributeManager : MonoBehaviour
    {

        [FormerlySerializedAs("_attributes")] [SerializeField] private List<EnvironmentAffector> _affectors = new List<EnvironmentAffector>();

        
        // Start is called before the first frame update
        void Start()
        {
            var attrs = GetComponentsInChildren<EnvironmentAffector>();

            for (var i = 0; i < attrs.Length; i++)
            {
                var breakLoop = false;
                
                foreach (EnvironmentAffector affector in _affectors)
                {
                    if (affector.attrType == attrs[i].attrType)
                        breakLoop = true;
                        break;
                }
                if(breakLoop)
                    break;
                _affectors.Add(attrs[i]);
            }
            
        }
        
        [PropertyOrder (0)]
        [Button(ButtonSizes.Large), GUIColor(1f, 1f, 1f)]
        void ResetAttributes()
        {
            _affectors.Clear();
        }

        public void UpdateAttribute(AttributeType type, float value)
        {
            foreach (EnvironmentAffector affector in _affectors)
            {
                if (type == affector.attrType)
                {
                    affector.CurrentValue += value;
                    affector.CheckThreshold();
                }
            }
        } 

    }
}