using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.EventSystems;
using Huenity;
using CS_Audio;
using MoreMountains.Feedbacks;
using uGUIPanelManager;
using Debug = UnityEngine.Debug;

namespace ChromaShift.Scripts.Managers
{
    [CreateAssetMenu(fileName = "GameState_EndGameplay", menuName = "GameState/EndGameplay")]
    public class EndGameplayState : BaseGameState
    {

        public EventSystem eventSystem;
        public MMFeedbacks gameOverFeedback;
        public override void EnterState()
        {
            base.EnterState();
            //HueManager.Instance.ResetLights();
            gameOverFeedback.PlayFeedbacks();
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void ExitState()
        {
            base.ExitState();
        }
        
    }
}