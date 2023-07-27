using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts.Managers
{
    //NOTE: SubStates are managed within their main state for flexibility. 
    public class GameStateManager : MonoBehaviour
    {

        // Current and previous states
        [SerializeField] private BaseGameState currentState;
        
        public BaseGameState CurrentState
        {
            get => currentState;
            set => currentState = value;
        }
        
        [SerializeField] private BaseGameState previousState;
        public BaseGameState PreviousState
        {
            get => previousState;
            set => previousState = value;
        }
        
        // State instances
        public BaseGameState AreaCompleteState;
        public BaseGameState CutsceneState;
        public BaseGameState EndGameplayState;
        public BaseGameState GameplayState;
        public BaseGameState GameSetupState;
        public BaseGameState InsightsState;
        public BaseGameState InventoryState;
        public BaseGameState JournalState;
        public BaseGameState LoadingState;
        public BaseGameState ApplicationStartState;
        public BaseGameState PauseState;
        public BaseGameState QuestState;
        public BaseGameState SaveLoadState;
        public BaseGameState SettingsMenuState;
        

        
        //SINGLETON PATTERN
        private static GameStateManager _instance;
        public static GameStateManager Instance {
            get 
            {
                if (_instance == null) 
                {
                    _instance = FindObjectOfType (typeof(GameStateManager)) as GameStateManager;

                    if (_instance == null)
                    {
                        GameObject go = new GameObject("_gameStateManager");
                        DontDestroyOnLoad(go);
                        _instance = go.AddComponent<GameStateManager>();
                    }
                }
			
                return _instance;
            }
        }

            private void Awake()
        {
            SwitchState(ApplicationStartState); // Set initial state
        }

        private void Update()
        {
            if (currentState != null)
            {
                currentState.UpdateState();
            }
        }

        public void SwitchState(BaseGameState newState)
        {
            if (currentState != null)
            {
                currentState.ExitState();
            }

            previousState = currentState;
            currentState = newState;

            if (currentState != null)
            {
                currentState.EnterState();
            }
        }
        
       
    }
}
