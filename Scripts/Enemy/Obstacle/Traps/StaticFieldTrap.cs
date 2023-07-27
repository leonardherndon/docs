using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class StaticFieldTrap : Hostile, ITrap<GameObject, Collider>
{

    [SerializeField]
    private int numChargesToDeploy = 100;
    [SerializeField]
    private float deployDelayTime = 1f;
    [SerializeField]
    private float deployInterval = 2f;
    
    [SerializeField]
    private GameObject targetEffect;
    public GameObject TargetEffect
    {
        get { return targetEffect; }

        set { targetEffect = value; }
    }
    [SerializeField]
    private GameObject payload;
    public GameObject Payload
    {
        get { return payload; }

        set { payload = value; }
    }

    [SerializeField]
    private GameObject[] objectContainer;


    public override void OnTriggerEnter(Collider other)
    {

        base.OnTriggerEnter(other);
        
        if (other.CompareTag("Player"))
        {
            Spawn();
            Debug.Log("On Trigger Firing.");
        }
    }


    public void Spawn()
    {
        StartCoroutine(LaunchProjectile());
    }

    public IEnumerator LaunchProjectile()
    {
        objectContainer = new GameObject[1];
        int i = 0;

        while (i < numChargesToDeploy)
        {
            if(i==0)
            {
                yield return new WaitForSeconds(deployDelayTime);
            }

            objectContainer[0] = FastPoolManager.GetPool(payload).FastInstantiate();
            objectContainer[0].transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
            GameManager.Instance.fastPoolObjects.Add(objectContainer[0]);

            //Set the Objects Reference Holder
            objectContainer[0].gameObject.GetComponent<Hostile>().MoC.startLane = MoC.startLane;
            objectContainer[0].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            //Sets the movableObject 
            objectContainer[0].gameObject.GetComponent<Hostile>().MoC.movableObject = objectContainer[0];
            objectContainer[0].GetComponent<Hostile>().CoC.CSM.ChromaShift(CoC.CSM.CurrentColor);

            //Sets the enemy starting current/destination lane			
            objectContainer[0].GetComponent<Hostile>().enemyLaneCurrent = MoC.startLane;
            objectContainer[0].GetComponent<Hostile>().enemyLaneDestination = MoC.startLane;
            objectContainer[0].GetComponent<Hostile>().CSM.ChromaShift(CSM.CurrentColor);

            i++;
            yield return new WaitForSeconds(deployInterval);
        }
       
        DisposeSelf();
    }

    public void DisposeSelf()
    {
        //GameObject currentDeathFx = Instantiate(deathFX[0], transform.position, Quaternion.identity);
        //currentDeathFx.transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
        Destroy(this.gameObject);
    }

}
