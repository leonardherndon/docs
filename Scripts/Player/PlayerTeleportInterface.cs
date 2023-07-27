using System.Collections;

namespace ChromaShift.Scripts.Player
{
    public interface PlayerTeleportInterface : PlayerAbilityInterface
    {
        float Distance { get; }
        float CoolDownTime { get; set; }
        
        
        void DoTeleport(TeleportDirectionEnum direction);

        IEnumerator TeleportCooldown();
    }
}