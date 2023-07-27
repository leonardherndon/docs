using System.Collections.Generic;
using ChromaShift.Scripts.Scriptables;
using UnityEditor;
using UnityEngine;

namespace ChromaShift.Scripts.Editor.Level
{
    /// <summary>
    /// The helper window that holds different buttons for basic crud work for our level editor.
    /// </summary>
    public class HelperWindow : EditorWindow
    {
        /// <summary>
        /// This will create the helper window.
        /// </summary>
        [MenuItem("Tools/ChromaShift/YuME/Helper Window")]
        private static void Init()
        {
            HelperWindow window = (HelperWindow) EditorWindow.GetWindow(typeof(HelperWindow), false, "YuME Helper");
        }
        
        /// <summary>
        /// This will create the buttons to show using the automatic layout
        /// </summary>
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            
            GUILayout.BeginVertical();
        
            if (GUILayout.Button("Clear level"))
            {
                ClearTiles(GameObject.Find("YuME_MapData"));
            }

            if (GUILayout.Button("Save level layout"))
            {
                SaveLevelLayout(GameObject.Find("YuME_MapData"));
            }
        
            GUILayout.EndVertical();
            
            GUILayout.EndArea();
        }

        private void SaveLevelLayout(GameObject yuMeMapData)
        {
            if (yuMeMapData.transform.childCount <= 0)
            {
                return;
            }

            LevelLayout levelLayout = ScriptableObject.CreateInstance<LevelLayout>();

            var layers = levelLayout.Layers;

            Debug.Log("Prefab type: " + PrefabUtility.GetPrefabType(yuMeMapData));
            Debug.Log("Prefab name: " + yuMeMapData.name);

            for (var i = 0; i < yuMeMapData.transform.childCount - 1; i++)
            {
                var obstacles = new List<BaseObstacle>();
                var child = yuMeMapData.transform.GetChild(i);

                Debug.Log("Working on child name: " + child.name);

                for (var j = 0; j < child.childCount; j++)
                {
                    BaseObstacle tmpBaseObject = CreateInstance<BaseObstacle>();
                    var tmpChild = child.transform.GetChild(j);
                    var tmpz = child.transform.position;
                    var tmpc = child.transform.localScale;

                    tmpBaseObject.Type = tmpChild.name;
//                    tmpBaseObject.X = tmpChild.position.x;
//                    tmpBaseObject.Y = tmpChild.position.y;
//                    tmpBaseObject.Z = tmpChild.position.z;
                    // TODO Update this to capture and save the prefab instead
                    obstacles.Add(tmpBaseObject);
                }

                layers.Add(child.name, obstacles);
            }

            Debug.Log("Layers count: " + layers.Count);
//            var tmpJson = JsonConvert.SerializeObject(layers);
//            Debug.Log("Json: " + tmpJson);
    }
        
        /// <summary>
        /// This will use the default YuME MapData GameObject that holds the layers and then prefabs within each layer.
        /// </summary>
        /// <param name="yuMeMapData"></param>
        private void ClearTiles(GameObject yuMeMapData)
        {
            if (yuMeMapData.transform.childCount <= 0)
            {
                return;
            }
        
            foreach (Transform child in yuMeMapData.transform)
            {
                int i = 0;
                while (child.childCount > 0)
                {
                    DestroyImmediate(child.GetChild(0).gameObject);
                }
            }
        }
    }
}