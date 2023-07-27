using Com.LuisPedroFonseca.ProCamera2D;
using CS_Audio;
using uGUIPanelManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChromaShift.Scripts.Managers
{
    [CreateAssetMenu(fileName = "GameState_Gameplay", menuName = "GameState/Gameplay")]
    public class GameplayState: BaseGameState
    {
        public PlayerShip playerShip;
        public GameObject levelDataObject;
        
        
        void Awake() {
            
            
		
        }
        public override void EnterState()
        {
            base.EnterState();
            playerShip = GameManager.Instance.playerShip;
            playerShip.warpClones.SetActive(false);
            GameUIManager.Instance.textBox.GetComponent<Text>().text = "";
            GameUIManager.Instance.textBox.FinalText = "";
            AudioManager.Instance.musicAudioSource.Stop ();
            GameManager.Instance.levelTimer.timerActive = true;
            if (!levelDataObject)
                levelDataObject = GameObject.Find ("LevelData");
            AudioManager.Instance.PlayClipWrap(0,0,GameManager.Instance.currentSceneDataObject.sceneMusic, true);
            PlayerInputController.Instance.RestoreControl();
            InputSystem.OnPauseGame += PauseGame;
            
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void ExitState()
        {
            base.ExitState();
            GameUIManager.Instance.textBox.GetComponent<Text>().text = "";
            GameManager.Instance.levelTimer.timerActive = false;
            PlayerInputController.Instance.CachedDisableControl();
            
            
        }
        
        public void PauseGame() {
            if (GameManager.Instance.gameState != GameStateType.Paused)
            {
                Debug.Log("Pausing Game");
                GameStateManager.Instance.SwitchState(GameStateManager.Instance.PauseState);
            }
            else
            {
                Debug.Log("Resuming Game");
                GameStateManager.Instance.SwitchState(GameStateManager.Instance.GameplayState);
            }
        }
    }
}