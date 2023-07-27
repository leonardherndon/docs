
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public interface IFactory
    {
        void AttachUpgrades(GameObject playerShip, IManager manager, UpgradePreferences upgradePreferences);
        void UpdateModifiers(IManager manager);
    }
}