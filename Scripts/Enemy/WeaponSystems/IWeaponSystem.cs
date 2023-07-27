using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Enemy.WeaponSystems
{

    public interface IWeaponSystem
    {

        GameObject[] Armaments { get; set; }

        GameObject[] DeployPoints { get; set; }

        AttackSequence[] AttackSequences { get; set; }
    }
}