using System.Collections.Generic;
using ChromaShift.Craft;
using Sirenix.Utilities;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class Factory : IFactory
    {
        private GameObject _playerShip;
        private IManager _manager;
        private IUpgrade[] _upgrades;
        private UpgradePreferences _upgradePreferences;

        public Factory(GameObject playerShip, IManager manager, ref UpgradePreferences upgradePreferences)
        {
            _playerShip = playerShip;
            _manager = manager;
            _upgradePreferences = upgradePreferences;
        }

        /// <summary>
        ///  Handles adding the upgrades needed to the Player Ship.
        /// </summary>
        /// <param name="playerShip"></param>
        /// <param name="manager"></param>
        /// <param name="upgradePreferences"></param>
        /// <returns></returns>
        public void AttachUpgrades(GameObject playerShip, IManager manager, UpgradePreferences upgradePreferences)
        {
            _upgradePreferences = upgradePreferences;
            RemoveUpgrades();
            var upgrades = new List<IUpgrade>();

            if (_upgradePreferences.HasHorizontal)
            {
                var horizontal = playerShip.AddComponent<Horizontal>();
                upgrades.Add(horizontal);
            }

            if (_upgradePreferences.HasVertical)
            {
                upgrades.Add(playerShip.AddComponent<Vertical>());
            }

            if (_upgradePreferences.HasTeleportIncrease)
            {
                upgrades.Add(playerShip.AddComponent<Teleport>());
            }

            if (_upgradePreferences.HasCoolDown)
            {
                upgrades.Add(playerShip.AddComponent<CoolDown>());
            }

            if (_upgradePreferences.HasNebula)
            {
                upgrades.Add(playerShip.AddComponent<NebulaProtection>());
            }

            if(_upgradePreferences.HasHardColor)
            {
                upgrades.Add(playerShip.AddComponent<HardColorProtection>());
            }

            if (_upgradePreferences.HasEarlyWarningSystem)
            {
                upgrades.Add(playerShip.AddComponent<EarlyWarningSystem>());
            }

            if (_upgradePreferences.HasCheckpointBeacon)
            {
                upgrades.Add(playerShip.AddComponent<CheckpointBeacon>());
            }
            
            _upgrades = upgrades.ToArray();
            SetModifiers(_upgrades, manager);
        }

        public void UpdateModifiers(IManager manager)
        {
            var upgrades = _upgrades ?? _playerShip.GetComponents<IUpgrade>();

            SetModifiers(upgrades, manager);
        }

        private void RemoveUpgrades()
        {
            var upgrades = _playerShip.GetComponents<IUpgrade>();

            foreach (var upgrade in upgrades)
            {
                upgrade.Remove();
            }
        }

        private void SetModifiers(IUpgrade[] upgrades, IManager manager)
        {
            foreach (var upgrade in upgrades)
            {
                switch (upgrade)
                {
                    case Horizontal h:
                        h.Modifier = manager.HorizontalModifier;
                        break;
                    case Vertical v:
                        v.Modifier = manager.VerticalModifier;
                        break;
                    case Teleport t:
                        t.Modifier = manager.TeleportModifier;
                        break;
                    case CoolDown c:
                        c.Modifier = manager.CoolDownModifier;
                        break;
                    case EarlyWarningSystem e:
                        e.Modifier = manager.EarlyWarningSystemDistance;
                        break;
                    case CheckpointBeacon c:
                        c.Modifier = manager.HasCheckpointBeacon;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}