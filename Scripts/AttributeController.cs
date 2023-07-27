using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Scripts.ObjectAttributeSystem
{
    [Serializable]
    public class AttributeStack : UnitySerializedDictionary<AttributeType, Vector3> {}
    public class AttributeModList :  UnitySerializedDictionary<AttributeType, List<AttributeModDataBlock>> {}
    public class AttributeController : MonoBehaviour
    {
        public AttributeRangeBlock rangeBlock;
        public AttributeStack attributes;
        
        public List<AttributeModDataBlock> attrModsHull;
        public List<AttributeModDataBlock> attrModsData;
        public List<AttributeModDataBlock> attrModsThermal;
        public List<AttributeModDataBlock> attrModsMental;
        public AttributeModList attributeModList;
        
        public delegate void OnAttributeChange(AttributeType type, float current,float max);
        public event OnAttributeChange AttributeChange = delegate { };
        
        public void Awake()
        {
            InitAttributeSystem();
        }
        
        public void InitAttributeSystem() 
        {
                ResetAttributeMinMax();
                
                attrModsHull.Clear();
                attrModsThermal.Clear();
                attrModsData.Clear();
                attrModsMental.Clear();
                
                attributeModList = new AttributeModList
                {
                    {AttributeType.HullIntegrity, attrModsHull},
                    {AttributeType.ThermalIntegrity, attrModsThermal},
                    {AttributeType.DataIntegrity, attrModsData},
                    {AttributeType.MentalIntegrity, attrModsMental},
                };
        }

        /*private void FixedUpdate()
        {
            CheckModList(attrModsData);
            CheckModList(attrModsHull);
            CheckModList(attrModsThermal);
            CheckModList(attrModsMental);
        }*/

        public void ResetAttributeMinMax()
        {            
            attributes = new AttributeStack
            {
                {AttributeType.HullIntegrity, new Vector3(rangeBlock.ATTRDEFAULT, rangeBlock.ATTRMIN, rangeBlock.ATTRMAX)},
                {AttributeType.ThermalIntegrity, new Vector3(rangeBlock.THERMDEFAULT, rangeBlock.THERMMIN, rangeBlock.THERMMAX)},
                {AttributeType.DataIntegrity, new Vector3(rangeBlock.ATTRDEFAULT, rangeBlock.ATTRMIN, rangeBlock.ATTRMAX)},
                {AttributeType.MentalIntegrity, new Vector3(rangeBlock.ATTRDEFAULT, rangeBlock.ATTRMIN, rangeBlock.ATTRMAX)},
            };
        }
        
        public void UpdateAttribute(AttributeType type, float value)
        {
            
            foreach(KeyValuePair<AttributeType,Vector3> attribute in attributes)
            {
                //Now you can access the key and value both separately from this attachStat as:
                Debug.Log(attribute.Key);
                Debug.Log(attribute.Value);

                if(attribute.Key == type) {
                    var statVal = attribute.Value.x;
                    var newVal = statVal + value;
                    var newVector3 = new Vector3(newVal,  attributes[attribute.Key].y,  attributes[attribute.Key].z);
                    attributes[attribute.Key] = newVector3;
                    
                }
            }
        } 
        
        public void SetAttribute(AttributeType type, float value)
        {
            
            foreach(KeyValuePair<AttributeType,Vector3> attribute in attributes)
            {
                //Now you can access the key and value both separately from this attachStat as:
                Debug.Log(attribute.Key);
                Debug.Log(attribute.Value);

                if(attribute.Key == type)
                {
                    var newVector3 = new Vector3(value,  attributes[attribute.Key].y,  attributes[attribute.Key].z);
                    attributes[attribute.Key] = newVector3;
                }
            }
        }
        
        public void SetAttributeMinMax(AttributeType type, float value, bool isMax = true)
        {
            
            foreach(KeyValuePair<AttributeType,Vector3> attribute in attributes)
            {
                //Now you can access the key and value both separately from this attachStat as:
                Debug.Log(attribute.Key);
                Debug.Log(attribute.Value);

                if(attribute.Key == type)
                {
                    var newVector3 = new Vector3();
                    if(isMax)
                        newVector3 = new Vector3(attributes[attribute.Key].x,  attributes[attribute.Key].y,  value);
                    else 
                        newVector3 = new Vector3(attributes[attribute.Key].x, value,   attributes[attribute.Key].z);
                    
                    attributes[attribute.Key] = newVector3;
                }
            }
        }
        
        public void ResetAttribute(AttributeType type)
        {
            
            foreach(KeyValuePair<AttributeType,Vector3> attribute in attributes)
            {
                //Now you can access the key and value both separately from this attachStat as:
                Debug.Log(attribute.Key);
                Debug.Log(attribute.Value);

                if(attribute.Key == type)
                {
                    var newVector3 = new Vector3(0,  attributes[attribute.Key].y,  attributes[attribute.Key].z);
                    attributes[attribute.Key] = newVector3;
                }
            }
        }

        public void UpdateAttributeMod(AttributeModDataBlock mod, float number)
        {
            mod.strength += number;
            //Debug.Log("Mod Update: " + mod.attrType + " | " + mod.originType + " | " + mod.baseStrength + " | " + mod.strength);
        }
        
        public void SetAttributeMod(AttributeModDataBlock mod, float number)
        {
            mod.strength = number;
        }

        //CalculateAttribute

        public void AddAttributeMod(AttributeModDataBlock mod)
        {
            var safeToAdd = false;
            safeToAdd = CompareModToList(mod, attributeModList[mod.attrType]);
            if (mod.notStackable)
            {
                if (safeToAdd)
                    DisableAllOtherModsOfType(mod);
                else
                    mod.isActive = false;
                
                attributeModList[mod.attrType].Add(mod);
            }
            else {
                if (!safeToAdd)
                {
                    mod.isActive = false;
                }
                attributeModList[mod.attrType].Add(mod);
            }

            CalculateMods(mod.attrType);
        }

        private void DisableAllOtherModsOfType(AttributeModDataBlock mod)
        {
            
            foreach (AttributeModDataBlock singleMod in attributeModList[mod.attrType])
            {
                if(singleMod.notStackable)
                    singleMod.isActive = false;
            }
        }
        
        public void EnableAllOtherModsOfType(AttributeModDataBlock mod)
        {
            
            foreach (AttributeModDataBlock singleMod in attributeModList[mod.attrType])
            {
                singleMod.isActive = true;
            }
        }
        
        public void RemoveAttributeMod(AttributeModDataBlock mod)
        {
            if (attributeModList[mod.attrType].Contains(mod))
            {
                attributeModList[mod.attrType].Remove(mod);
                if(mod.notStackable)
                    EnableAllOtherModsOfType(mod);
            }
            
            CalculateMods(mod.attrType);
        }

        

        public void CalculateMods(AttributeType type)
        {
            //Debug.Log("Attribute Controller - Calculate Mods Running");
            float result = type == AttributeType.ThermalIntegrity ? rangeBlock.THERMDEFAULT : rangeBlock.ATTRDEFAULT;
            foreach (AttributeModDataBlock mod in attributeModList[type])
            {
                if(mod.isActive)
                    result += mod.strength;
            }

            result = Mathf.Clamp(result, attributes[type].y, attributes[type].z);
            
            //Debug.Log("Result:" + result);
            attributes[type] = new Vector3(result, attributes[type].y,attributes[type].z);
            AttributeChange(type, result, attributes[type].z);
            //Debug.Log("Attribute Mod Calculation: " + type.ToString() + " | " + attributes[type].x);
        }

        public bool CompareModToList(AttributeModDataBlock mod, List<AttributeModDataBlock> list)
        {
            foreach (AttributeModDataBlock listMod in list)
            {
                if (mod.notStackable)
                    return false;
            }
            return true;
        }
        
        public float CalculateSpeedAdjustment(float baseSpeed)
        {
            float speedCalc = (attributes[AttributeType.ThermalIntegrity].x / 100f);
            
            baseSpeed += baseSpeed * speedCalc;
        
            return baseSpeed;
        }
        
        public float CalculateDamageReductionAdjustment(float baseReduction)
        {
            float result = baseReduction / (attributes[AttributeType.HullIntegrity].x / 100f);
            return result;
        }
        
        public float CalculateRecoveryAdjustment(float baseRecovery)
        {
            float result = baseRecovery;
            var attrPercent = ((attributes[AttributeType.DataIntegrity].x - 100f) / 100f);
            result -= baseRecovery * attrPercent;
        
            return result;
        }
        
        public float CalculateMentalAdjustment(float baseFocus)
        {
            float result = baseFocus;

            result += baseFocus * (attributes[AttributeType.MentalIntegrity].x / 100f);
        
            return result;
        }

    }
    
    
}