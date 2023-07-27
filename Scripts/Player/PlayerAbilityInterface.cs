namespace ChromaShift.Scripts.Player
{
    public interface PlayerAbilityInterface
    {
        float BatteryChargeRequired { get;}
        ILifeSystem PlayerLifeSystem { get; }

        void DoAbilityPrimary();

        void DoAbilitySecondary();

        void ExitAbilityPrimary();
        
        void ExitAbilitySecondary();
    }
}