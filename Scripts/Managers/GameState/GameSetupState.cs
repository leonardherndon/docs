using uGUIPanelManager;
using UnityEngine;

namespace ChromaShift.Scripts.Managers
{
    [CreateAssetMenu(fileName = "GameState_GameSetup", menuName = "GameState/GameSetup")]
    public class GameSetupState : BaseGameState
    {
        public override void EnterState()
        {
            base.EnterState();
            //GameUIManager.Instance.ResetSectorMap ();
        }

        public void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            base.ExitState();
        }
    }
}