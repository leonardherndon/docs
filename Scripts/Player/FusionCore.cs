using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts.Player
{
    [System.Serializable]
    public class FusionCore : IEquatable<FusionCore>
    {
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
        
            FusionCore objAsPart = obj as FusionCore;
        
            if (objAsPart == null) return false;
        
            return Equals(objAsPart);
        }

        public bool Equals(FusionCore other)
        {
            if (other == null) return false;
            return other.ColorIndex == ColorIndex;
        }

        [SerializeField] private bool isActive;
        public bool IsActive
        {
            get => isActive;
            set => isActive = value;
        }
        
        [SerializeField] private bool doesRegen;
        public bool DoesRegen
        {
            get => doesRegen;
            set => doesRegen = value;
        }

        [SerializeField] private float regenTime;
        public float RegenTime
        {
            get => regenTime;
            set => regenTime = value;
        }
        
        [SerializeField] private float regenTimeMax;
        public float RegenTimeMax
        {
            get => regenTimeMax;
            set => regenTimeMax = value;
        }

            [SerializeField] private GameColor colorIndex;
        public GameColor ColorIndex
        {
            get => colorIndex;
            set => colorIndex = value;
        }

        public FusionCore(GameColor color, bool active, bool regen, float max)
        {
            colorIndex = color;
            isActive = active;
            doesRegen = regen;
            regenTimeMax = max;
        }

        public IEnumerator Regeneration()
        {
            Debug.Log("Going to check if regeneration is possible.");
            if(!doesRegen || isActive)
                yield break;

            Debug.LogFormat("Going to wait for {0} seconds to regenerate for ColorIndex {1}", regenTimeMax, colorIndex);
            yield return new WaitForSeconds(regenTimeMax);
            
            isActive = true;
            regenTime = 0;
            Debug.LogFormat("Done regeneration for ColorIndex {0}", colorIndex);
        }
    }
}