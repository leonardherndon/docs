using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class BeamCannonTrap : Hostile, ITrap<GameObject, Collider>
{
    [Header("[BEAM CANNON TRAP]")]
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
    private GameObject targetEffect;

    private bool hasDeployed = false;
    
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
        if (hasDeployed)
            return;
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            
            if((other.name == "TopBumper") || (other.name == "BottomBumper")) {
                return;
            }

            if (playerShip.isShieldActive)
            {
                bool detected =
                    ColorManager.Instance.isColorWeaker(GameManager.Instance.playerShip.gameObject, this.gameObject);
                
                if (detected)
                {
                    Spawn();
                    hasDeployed = true;
                }
            }
            else
            {
                Spawn();
                hasDeployed = true;
            }
        }
    }
    
    public void OnTriggerStay(Collider other)
    {
        if (hasDeployed)
            return;

        if (other.CompareTag("Player"))
        {
            
            if((other.name == "TopBumper") || (other.name == "BottomBumper")) {
                return;
            }

            if (playerShip.isShieldActive)
            {
                bool detected =
                    ColorManager.Instance.isColorWeaker(GameManager.Instance.playerShip.gameObject, this.gameObject);
                
                if (detected)
                {
                    Spawn();
                    hasDeployed = true;
                }
            }
        }
    }

    public void Spawn()
    {
        GameObject TargetIcon = Instantiate(targetEffect, new Vector3(GameManager.Instance.playerShip.transform.position.x, GameManager.Instance.playerShip.transform.position.y, GameManager.Instance.playerShip.transform.position.z - 1f), Quaternion.identity);
        TargetIcon.transform.parent = GameManager.Instance.playerShip.transform;
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

//            Debug.Log("Deploying Beam Cannon: " + i);
            GameObject currentLaser = Instantiate(payload, GameManager.Instance.playerShip.transform.position, Quaternion.identity);
            currentLaser.transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
            
            i++;
            yield return new WaitForSeconds(deployInterval);
        }

        DisposeSelf();
    }

    public void DisposeSelf()
    {
        Destroy(this.gameObject);
    }

}
