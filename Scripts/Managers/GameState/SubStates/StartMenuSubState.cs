using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine.EventSystems;
using Huenity;
using CS_Audio;
using uGUIPanelManager;
using Debug = UnityEngine.Debug;

namespace ChromaShift.Scripts.Managers
{
    [CreateAssetMenu(fileName = "GameSubState_StartMenu", menuName = "GameState/SubState/StartMenu")]
    public class StartMenuSubState : BaseGameSubState, ISubState

    {
    public ProCamera2D mainCamera;
    public EventSystem eventSystem;

    public void EnterState()
    {

        if (GameManager.Instance.isTesting == true)
        {
            GameManager.Instance.removeTest = true;
            GameManager.Instance.SetLevelToBeLoaded(null);
            GameManager.Instance.LoadSceneWrap();
        }

        mainCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
        mainCamera.GetComponent<Camera>().enabled = false;

        eventSystem.SetSelectedGameObject(GameUIManager.Instance.UIElementsSplashScreen[2].gameObject);
        // GameUIManager.Instance.UIElementsSplashScreen[2].GetComponent<Button>().Select();
        // GameUIManager.Instance.UIElementsSplashScreen[2].GetComponent<Button>().OnSelect(null);

    }
    }
}