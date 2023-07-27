using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public WaypointSet[] waypointSets;


    public void ConvertPointsToScreen()
    {
        for (int i = 0; i < waypointSets.Length; i++)
        {
            for (int x = 0; x < waypointSets[i].waypoints.Length; x++)
            {
                waypointSets[i].waypoints[x] = GameManager.Instance.mainCamera.GetComponent<Camera>()
                    .WorldToScreenPoint(waypointSets[i].waypoints[x]);
            }
        }   
    }
}

[System.Serializable]
public struct WaypointSet
{
    public string setID;
    public Vector3[] waypoints;
    public bool isLocal;
}
