namespace ChromaShift.Scripts.Player.Upgrade
{
    public interface IManager
    {
        float VerticalModifier { get; set; }
        float HorizontalModifier { get; set; }
        float TeleportModifier { get; set; }
        float CoolDownModifier { get; set; }
        float EarlyWarningSystemDistance { get; set; }
        bool HasCheckpointBeacon { get; set; }
        bool HasWhiteColorAbility { get; set; }
        void Awake();
        PlayerShip GetPlayerShip();
        IFactory GetFactory();
    }
}