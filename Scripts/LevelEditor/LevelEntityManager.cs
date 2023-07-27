using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class LevelEntityManager : MonoBehaviour {

    public GameObject oldObject;
    public string objectTag;
    public GameObject replacementObject;
    public bool testRun = true;

    [BoxGroup("Object Replacer")]
    [GUIColor(1, 0.3f, 0, 1)]

    [Button(ButtonSizes.Gigantic)]
    private void ReplaceObject() { ReplaceSourceObject(); }


    public void ReplaceSourceObject()
    {
        GameObject[] spawns;
        spawns = GameObject.FindGameObjectsWithTag(objectTag);
        Debug.Log("Number of Spawners Gathered: " + spawns.Length);
        int thisCount = 0;
        foreach (GameObject spawn in spawns)
        {
            ObjectSpawner oSpawn;
            
            Debug.Log("Spawn Name: " + spawn.name);
            if (spawn.GetComponent<ObjectSpawner>() != null)
            {
                oSpawn = spawn.GetComponent<ObjectSpawner>();

                Debug.Log("oSpawn Name: " + oSpawn.sourcedObject.name);
                if (oSpawn.sourcedObject.name == oldObject.name)
                {
                    thisCount++;
                    if(!testRun)
                        oSpawn.sourcedObject = replacementObject;
                    Debug.Log("Adding 1");
                }
                
            }
        }
        Debug.Log("Number of Spawners spawning [" + oldObject.name + "]: " + thisCount);
    }
}
