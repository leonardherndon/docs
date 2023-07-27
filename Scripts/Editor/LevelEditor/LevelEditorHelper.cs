using HelpMePlace;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class LevelEditorHelper
{
   
    
    // register an event handler when the class is initialized
    static LevelEditorHelper()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    
    
    private static void LogPlayModeState(PlayModeStateChange state)
    {
        //Debug.Log(state);
        if (state == PlayModeStateChange.EnteredEditMode && GameManager.Instance.cachedScenePath != null)
        {
            if (GameManager.Instance.cachedScenePath == null)
                return;
            EditorSceneManager.OpenScene(GameManager.Instance.cachedScenePath, OpenSceneMode.Additive);
            
            Debug.Log("Attempting to Find Layout Object.");
            GameObject go = GameObject.Find("Layout");
            if (go == null)
            {
                Debug.Log("Attempting to Find ActiveObjects.");
                go = GameObject.Find("ActiveObjects");
                if (go == null)
                {
                    Debug.LogError("This Level does not contain Layout nor ActiveObjects GameObjects Needed.");
                    return;
                }
            }
            
            Selection.activeGameObject = go;
            
            var HMP = GameObject.Find("HelpMePlace");
            if (HMP != null)
            {
                Debug.Log("Attempting to Load HMP.");
                GameManager.Instance.cachedHMP = HMP;
            }
            else
            {
                Debug.Log("No HMP to load.");
            }
                
            
            EditorWindow hierarchy = EditorWindow.GetWindow(typeof(EditorWindow), false, "Hierarchy", true);
            Event keyPress = new Event {keyCode = KeyCode.RightArrow, type = EventType.KeyDown };
            hierarchy.SendEvent(keyPress);

            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        }

        if (state == PlayModeStateChange.ExitingEditMode )
        {
            EditorSceneManager.SaveOpenScenes();
            var allScenes = SceneManager.GetAllScenes();
            if(allScenes.Length > 1)
                GameManager.Instance.cachedScenePath = allScenes[1].path;
            else if(allScenes.Length <= 1)
                GameManager.Instance.cachedScenePath = null;
            
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                //SceneManager.UnloadSceneAsync(i);
                EditorSceneManager.CloseScene(SceneManager.GetSceneByBuildIndex(i), false);
            }
        }
    }
    
    

    [MenuItem("Tools/ChromaShift/Helper/Cache Scene")]
    static void CacheScene()
    {
        Debug.Log("Attempting to Cache Scene");
        var Scenes = SceneManager.GetAllScenes();
        Debug.Log("Scene Count: "+ Scenes.Length);
        GameManager.Instance.cachedScenePath = Scenes[1].path;

    }

    [MenuItem("Tools/ChromaShift/Helper/Clear Cached Scene")]
    static void ClearCachedScene()
    {
        GameManager.Instance.cachedScenePath = null;
    }
    
    [MenuItem("Tools/ChromaShift/HMP/Cache HMP")]
    static void CacheHMP()
    {
        GameManager.Instance.cachedHMP = (GameObject)Selection.activeObject;
    }
    
    [MenuItem("Tools/ChromaShift/HMP/Clear HMP")]
    static void ClearHMP()
    {
        GameManager.Instance.cachedHMP = null;

    }
    
    [MenuItem("Tools/ChromaShift/HMP/Toggle HMP")]
    static void ToggleHMP()
    {
        if (!GameManager.Instance.cachedHMP.GetComponent<HelpMePlaceSystem>().enabled)
            GameManager.Instance.cachedHMP.GetComponent<HelpMePlaceSystem>().enabled = true;
        else 
            GameManager.Instance.cachedHMP.GetComponent<HelpMePlaceSystem>().enabled = false;
    }
}