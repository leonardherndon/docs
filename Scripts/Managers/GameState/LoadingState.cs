using CS_Audio;
using uGUIPanelManager;
using UnityEngine;

namespace ChromaShift.Scripts.Managers
{
    [CreateAssetMenu(fileName = "GameState_Loading", menuName = "GameState/Loading")]
    public class LoadingState : BaseGameState 
    {
        public override void EnterState()
        {
            base.EnterState();
            AudioManager.Instance.StopPlayingClip(3);
            //HueManager.Instance.shouldPulse = false;
            //HueManager.Instance.SetAllLights(new HueLightState() { effect = Effect.colorloop });

        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void ExitState()
        {
            base.ExitState();
            //HueManager.Instance.SetAllLights(new HueLightState() { effect = Effect.none });
            //UIManager.HideUiElement ("Panel_LoadGame", "CS_LoadGame");
        }
    }
}