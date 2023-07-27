using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.ObjectAttributeSystem;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts
{
	[CreateAssetMenu(menuName = "ChromaShift/DataBlocks/StatusEffect")]
	public class StatusEffectDataBlock : ScriptableObject
	{
		[FormerlySerializedAs("statusEffect")] [FormerlySerializedAs("statusEffectGroup")] [FormerlySerializedAs("nebulaType")] public StatusEffectType statusEffectType;
		public EffectApplicationType applicationType = EffectApplicationType.Null;
		public int limit = 0;
		public float threshold = 0;
		public float tickRate = 0;
		public List<AttributeModDataBlock> attrModReference;
		public List<AttributeModDataBlock> attrMods;
		public GameObject GUIPrefab;
	}
}