using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace ChromaShift.Enemy.WeaponSystems
{
    
public class WeaponSystem : SerializedMonoBehaviour, IWeaponSystem
{
    
    private GameObject bossObject;

    public Animator animator;

    public AnimationClip[] animationClips;

    [SerializeField] private GameObject[] _armaments;
    
    [SerializeField] private GameObject[] _deployPoints;
    
    public AttackSequence[] _sequences;


    public GameObject[] Armaments
    {
        get => _armaments;
        set => _armaments = value;
    }

    public GameObject[] DeployPoints
    {
        get => _deployPoints;
        set => _deployPoints = value;
    }

    public AttackSequence[] AttackSequences
    {
        get => _sequences;
        set => _sequences = value;
    }


    /// <summary>
    ///     Process Sequence will run through each attack unit to deploy what is necessary. There's a delay for the start of the sequence as well as for each deployment. 
    /// </summary>
    public IEnumerator ProcessSequence(int sequenceIndex)
    {
        AttackSequence currentSequence = _sequences[sequenceIndex];

        foreach (AttackUnit unit in currentSequence.attackUnits)
        {
            DeployArmament(unit.armamentIndex, unit.deployPointIndex,unit.gameColor, unit.localToParent);
            
            yield return new WaitForSeconds(unit.cooldownDelay); //If Zero it will start the next shot immediately.
        }
    }
    
    /// <summary>
    ///     DeployArmament will take the given armament and spawn the prefab at the given deploy point location.
    /// </summary>
    private void DeployArmament(int armamentIndex, int deployPointIndex, GameColor gameColor, bool local)
    {
        Transform spawn = GameManager.Instance.spawnedObjectsHolder.transform;
        
        if(local)
            spawn = transform;
        
        var payload = Instantiate(
            _armaments[armamentIndex],
            _deployPoints[deployPointIndex].transform.position,
            Quaternion.identity,
            spawn
        ) as GameObject;
        
        Debug.Log("Weapon Deployed: " + payload.gameObject.name);

        var rends = payload.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in rends)
        {
            rend.material.SetColor ("_EmissionColor", ColorManager.Instance.GetColor(gameColor));
        }

        CollisionController CoC = payload.GetComponent<CollisionController>();
        
        if (CoC)
        {
            var cIndex = gameColor;

            CoC.CSM.ChromaShift(cIndex);
        }

    }
}

}