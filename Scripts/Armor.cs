using ChromaShift.Scripts.ObjectAttributeSystem;
using UnityEngine;

namespace ChromaShift.Scripts
{
    /// <summary>
    /// Base class to use for the armor. We just need to create some prefabs with predetermined values and attach it to the player ship, or the any component, as long as it has an AttributeController.
    ///
    /// TODO: Refactor this into a better namespace.
    /// </summary>
    public class Armor: MonoBehaviour
    {
        private AttributeController _attributeController;
        public float HullIntegrityAdjustment = 17.0f;
        public float ThermalIntegrityAdjustment = 13.0f;
        private void Start()
        {
            var curHI = _attributeController.attributes[AttributeType.HullIntegrity];
            var curTI = _attributeController.attributes[AttributeType.ThermalIntegrity];

            Vector3 updatedHI = new Vector3(curHI.x + HullIntegrityAdjustment, curHI.y, curHI.z);
            Vector3 updatedTI = new Vector3(curTI.x + ThermalIntegrityAdjustment, curTI.y, curTI.z);

            _attributeController.attributes[AttributeType.HullIntegrity] = updatedHI;
            _attributeController.attributes[AttributeType.ThermalIntegrity] = updatedTI;
        }

        private void OnDestroy()
        {
            var curHI = _attributeController.attributes[AttributeType.HullIntegrity];
            var curTI = _attributeController.attributes[AttributeType.ThermalIntegrity];

            Vector3 updatedHI = new Vector3(curHI.x - HullIntegrityAdjustment, curHI.y, curHI.z);
            Vector3 updatedTI = new Vector3(curTI.x - ThermalIntegrityAdjustment, curTI.y, curTI.z);

            _attributeController.attributes[AttributeType.HullIntegrity] = updatedHI;
            _attributeController.attributes[AttributeType.ThermalIntegrity] = updatedTI;
        }

        private void Awake()
        {
            _attributeController = gameObject.GetComponent<AttributeController>();
        }
    }
}