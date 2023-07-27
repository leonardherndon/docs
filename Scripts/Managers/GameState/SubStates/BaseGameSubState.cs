using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.EventSystems;
using Huenity;
using CS_Audio;
using Sirenix.Serialization;
using uGUIPanelManager;
using Debug = UnityEngine.Debug;

namespace ChromaShift.Scripts.Managers
{
    public class BaseGameSubState : BaseGameState
    {
        [OdinSerialize] public GameSubStateType StateSubType { get; set; }
        [OdinSerialize] public IHasSubStates parentState { get; set; }
        public bool isSubState = true;
        public virtual void EnterSubState()
        {
            UIObjectName = GameUIManager.Instance.GetUIPanelFromGameObject(AssociatedUIObject);
            uGUIManager.SetPanelState(UIObjectName, PanelState.Show, additional: true, queued: false, instant: true);
            
        }
        
        public virtual void UpdateSubState()
        {
            throw new System.NotImplementedException();
        }
        
        public virtual void ExitSubState()
        {
            uGUIManager.SetPanelState(UIObjectName, PanelState.Hide, additional: true, queued: false, instant: true);
        }
        
    }
}