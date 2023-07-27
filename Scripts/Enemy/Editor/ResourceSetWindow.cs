using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace ChromaShift.Scripts.Enemy
{
    public class ResourceSetWindow : OdinEditorWindow
    {
        public string fileName = "resourceSet00";
        public Queue<IResourceSet> ResourceSets;

        private ResourceCollection _resourceCollection;
        private string rootPath = "Assets";

        [MenuItem("Tools/ChromaShift/Enemies/ResourceSet")]
        private static void OpenWindow()
        {
            GetWindow<ResourceSetWindow>().Show();
        }

//        public void OnEnable()
//        {
//            rootPath = string.Concat(AssetDatabase.Path, "/ChromaShift/Resources/ResourceSets");
//        }

        [HorizontalGroup]
        [Button(ButtonSizes.Medium)]
        public void Save()
        {
            var altPath = EditorUtility.SaveFilePanelInProject("Alt title", fileName, "asset", "Some Message");

            var tmp = ScriptableObject.CreateInstance<ResourceCollection>();
            List<IResourceSet> list = new List<IResourceSet>();

            foreach (var resourceSet in ResourceSets)
            {
                list.Add(resourceSet);
            }
            
            tmp.data = list;
                
            AssetDatabase.CreateAsset(tmp, altPath);
        }

        [HorizontalGroup]
        [Button(ButtonSizes.Medium)]
        public void Open()
        {
            var path = EditorUtility.OpenFilePanel("Select Resource set", rootPath, "asset");
            fileName = Path.GetFileName(path);
            var altPath = "Assets";
            
            if (path.StartsWith(Application.dataPath)) {
                altPath =  "Assets" + path.Substring(Application.dataPath.Length);
            }
            
            var tmp = AssetDatabase.LoadAssetAtPath<ResourceCollection>(altPath);
            
            if (tmp == null)
            {
                Debug.LogErrorFormat("Issue trying to load the ResourceSet from {0}. It's null.'", altPath);
                return;
            }

            Queue<IResourceSet> queue = new Queue<IResourceSet>();

            foreach (var resourceSet in tmp.data)
            {
                queue.Enqueue(resourceSet);
            }

            ResourceSets = queue;
        }
    }
}