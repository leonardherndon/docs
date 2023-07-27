using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts
{
    [CreateAssetMenu(menuName = "ChromaShift/DataBlocks/Damage")]
    public class DamageDataBlock : ScriptableObject
    {
        [FormerlySerializedAs("damageStatusApplicationType")] public DamageApplicationType damageApplicationType = DamageApplicationType.Ignore;
        [Tooltip("0 = Dampened, 1 = Normal, 2 = Amplified")]
        public float[] damageAmount = new float[3] {1f,50f,90f};
        public int limit = 0;
        public float tickRate = 0;
        public float startDelay = 0;
    }
}