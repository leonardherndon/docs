using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using ChromaShift.Scripts.ObjectAttributeSystem;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ChromaShift/DataBlocks/LifeSystemData")]
public class LifeSystemData : SerializedScriptableObject
{
    public float StartingHP;
    public float TotalHP;
    public bool isPlayer;
    public bool doesRegenHealth = false;
    public float[] regenRate = new float[4] {0.05f,0.25f,0.5f,0.75f};
    
    [Header("[SPECIAL FX]")]
    public MMFeedbacks feedbackTakeDamage;
    public MMFeedbacks deathEffect;
    
    
}
