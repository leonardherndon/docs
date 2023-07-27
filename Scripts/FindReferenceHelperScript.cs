using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindReferenceHelperScript : MonoBehaviour
{
    public GameObject target; // The GameObject you're checking for references to
    // Start is called before the first frame update
    void Start()
    {
        var allObjects = FindObjectsOfType<MonoBehaviour>();
        foreach (var obj in allObjects)
        {
            if (obj.gameObject == target && obj is FindReferenceHelperScript) // Add this check here
                continue;
            
            var fields = obj.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(GameObject))
                {
                    GameObject value = field.GetValue(obj) as GameObject;
                    if (value == target)
                    {
                        Debug.Log(obj.name + " has a reference to " + target.name);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

