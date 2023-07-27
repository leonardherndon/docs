using uGUIPanelManager;
using UnityEngine;

namespace ChromaShift.Scripts.Managers
{
    public class PauseMenuState: BaseGameState
    {
        public override void EnterState()
        {
            base.EnterState();
            GameManager.Instance.rootClock.localTimeScale = 0;

        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void ExitState()
        {
            base.ExitState();
            GameManager.Instance.rootClock.localTimeScale = 1;
        }
    }
}