using DG.Tweening;
using uGUIPanelManager;
using UnityEngine;
using UnityEngine.UI;

namespace ChromaShift.Scripts.Managers
{
    [CreateAssetMenu(fileName = "GameSubState_Splash", menuName = "GameState/SubState/Splash")]
    public class SplashScreenSubState : BaseGameSubState, ISubState
    {
        
        [SerializeField] private GameObject SplashLogo;
        [SerializeField] private float splashFadeTime = 7f;
        
        public override void EnterSubState()
        {
            base.EnterSubState();
            UIObjectName = GameUIManager.Instance.GetUIPanelFromGameObject(AssociatedUIObject);
            uGUIManager.SetPanelState(UIObjectName, PanelState.Show, additional: true, queued: false, instant: true);
            
            //HueManager.Instance.ResetLights();
            
            SplashLogo.GetComponent<Image>().DOFade(1,splashFadeTime).OnComplete(NextState);
        }

        public void NextState()
        {
            parentState.SwitchSubState(parentState.SubStateList[1]); //Switches to the Next SubState which should be the StartMenu
        }
    }
}