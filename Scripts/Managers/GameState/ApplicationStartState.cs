using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public class ApplicationStartState : BaseGameState, IHasSubStates
    {
        
        [OdinSerialize] public List<BaseGameSubState> SubStateList { get; set; }
        [SerializeField] private BaseGameSubState currentSubState;
        public BaseGameSubState CurrentSubState
        {
            get => currentSubState;
            set => currentSubState = value;
        }
        
        
        public override void EnterState()
        {
        
            base.EnterState();
            if (SubStateList.Count > 0)
            {
                InitSubState(SubStateList);
                SwitchSubState(SubStateList[0]);
            }
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            base.ExitState();
            EndSubStates();
        }
        
        public void SwitchSubState(BaseGameSubState newSubState)
        {
            if (currentSubState != null)
            {
                currentSubState.ExitSubState();
            }
            
            currentSubState = newSubState;

            if (currentSubState != null)
            {
                currentSubState.EnterSubState();
            }
        }

        public void InitSubState(List<BaseGameSubState> SubStateList)
        {
            foreach (BaseGameSubState subState in SubStateList)
            {
                subState.parentState = this;
            }
        }

        public void EndSubStates()
        {
            if (currentSubState != null)
            {
                currentSubState.ExitSubState();
            }
        }
    }
}