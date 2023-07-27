using Sirenix.OdinInspector;
using Sirenix.Serialization;
using uGUIPanelManager;
using UnityEngine;

namespace ChromaShift.Scripts.Managers
{
    public class BaseGameState: SerializedMonoBehaviour, IGameState
    {
        [OdinSerialize] public GameStateType StateType { get; set; }
        private string UIid;
        public GameObject AssociatedUIObject;
        [SerializeField] protected string UIObjectName; 
        public virtual void EnterState()
        {
            UIid = GameUIManager.Instance.GetUIPanelFromGameObject(AssociatedUIObject);
            UIObjectName = GameUIManager.Instance.GetUIPanelFromGameObject(AssociatedUIObject);
            uGUIManager.SetPanelState(UIObjectName, PanelState.Show, additional: false, queued: true, instant: true);
        }

        public virtual void UpdateState()
        {
            
        }

        public virtual void ExitState()
        {
            uGUIManager.SetPanelState(UIObjectName, PanelState.Hide, additional: false, queued: true, instant: true);
        }
    }
}