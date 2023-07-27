using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionCoreEjector : MonoBehaviour {

    private Hostile hostileObject;
    public List<GameObject> fusionCores = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //Hostile.OnHostileDestroyed += EjectFusionCores;
        hostileObject = gameObject.GetComponent<Hostile>();
        LoadCores();

    }

    private void LoadCores()
    {
        fusionCores.Clear();
        switch (hostileObject.CSM.CurrentColor)
        {
            case GameColor.Red: //Red
                fusionCores.Add(GameManager.Instance.fusionCores[0]);
                break;
            case GameColor.Green: //Green
                fusionCores.Add(GameManager.Instance.fusionCores[1]);
                break;
            case GameColor.Blue: //Blue
                fusionCores.Add(GameManager.Instance.fusionCores[2]);
                break;
            case GameColor.Yellow: //Yellow
                fusionCores.Add(GameManager.Instance.fusionCores[0]);
                fusionCores.Add(GameManager.Instance.fusionCores[1]);
                break;
            case GameColor.Purple: //Purple
                fusionCores.Add(GameManager.Instance.fusionCores[0]);
                fusionCores.Add(GameManager.Instance.fusionCores[2]);
                break;
            case GameColor.Cyan: //Cyan
                fusionCores.Add(GameManager.Instance.fusionCores[1]);
                fusionCores.Add(GameManager.Instance.fusionCores[2]);
                break;
            case GameColor.White: //White
                fusionCores.Add(GameManager.Instance.fusionCores[0]);
                fusionCores.Add(GameManager.Instance.fusionCores[1]);
                fusionCores.Add(GameManager.Instance.fusionCores[2]);
                break;
        }
    }

    public void EjectFusionCores()
    {
        foreach (GameObject core in fusionCores)
        {
            Vector3 spawnLocation = new Vector3(transform.position.x + Random.Range(5f,10f), transform.position.y + Random.Range(-8f, 8f), transform.position.z);
            GameObject currentCore = Instantiate(core, spawnLocation, Quaternion.identity);
            currentCore.transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
        }
    }
}
