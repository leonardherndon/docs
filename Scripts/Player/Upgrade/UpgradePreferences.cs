using System;
using Sirenix.OdinInspector;

namespace ChromaShift.Scripts.Player.Upgrade
{
    [Serializable]
    public struct UpgradePreferences
    {
        public bool HasForwardTeleport;
        public bool HasTeleportIncrease;
        public bool HasVertical;
        public bool HasHorizontal;
        public bool HasNebula;
        public bool HasHardColor;
        public bool HasCoolDown;
        public bool HasEarlyWarningSystem;
        public bool HasCheckpointBeacon;

        public bool HasWhiteColorAbility;
    }
}