using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class LaserTrap : Hostile, ITrap<GameObject, Collider>
{
    [Header("[LASER TRAP]")]
    [SerializeField]
    private GameObject payload;
    public GameObject Payload
    {
        get { return payload; }

        set { payload = value; }
    }

    [SerializeField]
    private int numChargesToDeploy = 1;
    [SerializeField]
    private float deployDelayTime = 1f;
    [SerializeField]
    private float deployInterval = 0.03f;
    [SerializeField]
    public Transform laserTip;
    [SerializeField]
    private GameObject targetEffect;
    public GameObject TargetEffect
    {
        get { return targetEffect; }

        set { targetEffect = value; }
    }

    [SerializeField]
    private GameObject[] objectContainer;
    [SerializeField]
    

    public override void OnTriggerEnter(Collider other)
    {

        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            //Forcing the laser to always Shoot no matter the color you are. 
           // bool detected = ColorManager.Instance.isColorWeaker(GameManager.Instance.playerShip.gameObject, this.gameObject);
            //if (detected)
            //{
               
                GetComponentInChildren<Animator>().SetBool("Activated", true);
            //}
        }
    }
    //public void FixedUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        Debug.Log("Firing Laser!");
    //        this.GetComponentInChildren<Animator>().SetBool("Activated", true);
    //    }
    //}

    public void Spawn()
    {
        GameObject TargetIcon = Instantiate(targetEffect, new Vector3(GameManager.Instance.playerShip.transform.position.x, GameManager.Instance.playerShip.transform.position.y, GameManager.Instance.playerShip.transform.position.z - 1f), Quaternion.identity);
        TargetIcon.transform.parent = GameManager.Instance.playerShip.transform;
        TargetIcon.transform.parent = null;
        StartCoroutine(LaunchProjectile());
  
    }

    public IEnumerator LaunchProjectile()
    {

        int i = 0;

        while (i < numChargesToDeploy)
        {
            if (i == 0)
            {
                yield return new WaitForSeconds(deployDelayTime);
            }

            //Debug.Log("Deploying Laser: " + i);
            GameObject currentPaylod = Instantiate(payload, laserTip.position, Quaternion.identity, gameObject.transform);

            i++;
            yield return new WaitForSeconds(deployInterval);
        }
        
        yield return new WaitForSeconds(deployInterval);
        DisposeSelf();
    }

    public void DisposeSelf()
    {
        Instantiate(deathFX[0], transform.position, Quaternion.identity);
        //Destroy(this.gameObject);
    }
}
