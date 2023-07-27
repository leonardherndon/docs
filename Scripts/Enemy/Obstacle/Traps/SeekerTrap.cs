using UnityEngine;
using System.Collections;
using CS_Audio;
using DG.Tweening;

public class SeekerTrap : Hostile, ITrap<GameObject, Collider>
{
    [Header("[SEEKER TRAP]")]
    [SerializeField]
    private GameObject payload;
    public GameObject Payload
    {
        get { return payload; }

        set { payload = value; }
    }

    [SerializeField]
    private int numChargesToDeploy = 8;

    [SerializeField] private int deployedCharges;
    [SerializeField]
    private float deployDelayTime = 0.25f;
    [SerializeField]
    private float deployInterval = 0.03f;
    [SerializeField]
    private GameObject targetEffect;

    public GameObject TargetEffect
    {
        get {return targetEffect; }

        set { targetEffect = value;}
    }


    public override void OnTriggerEnter (Collider other) {

        base.OnTriggerEnter(other);

        if (other.CompareTag("Player")) {
            //bool detected = ColorManager.Instance.isColorWeaker(GameManager.Instance.playerShip.gameObject, this.gameObject);
            //if (detected) {
                Spawn();
            //}
        }
	}

    public void Spawn()
    {
        GameObject TargetIcon = Instantiate(targetEffect, new Vector3(GameManager.Instance.playerShip.transform.position.x, GameManager.Instance.playerShip.transform.position.y, GameManager.Instance.playerShip.transform.position.z - 20f), Quaternion.identity);
        TargetIcon.transform.parent = GameManager.Instance.playerShip.transform;
        
        //ACTIVATION ANIMATION
        var mySequence = DOTween.Sequence();
        mySequence
            .Append(transform.DORotate(new Vector3(0, 60, 360), 0.5f, RotateMode.WorldAxisAdd)
                .SetEase(Ease.InBack))
            .Append(transform.DOLocalRotate(new Vector3(0, 0, 90), 0.25f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear).SetLoops(20).OnStepComplete(LaunchHelper).OnComplete(DisposeSelf));
        mySequence.Play(); 
        AudioManager.Instance.PlayClipWrap(5, 18);
    }

    public void LaunchHelper()
    {
        StartCoroutine(LaunchProjectile());
    }
    
    public IEnumerator LaunchProjectile()
    {

        if (deployedCharges < numChargesToDeploy)
        {
            //Debug.Log("Deploying Seeker: " + i);
            GameObject currentPaylod = Instantiate(payload, transform.position, Quaternion.identity);
            currentPaylod.transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
            deployedCharges++;
        }
        return null;
    }

    public void DisposeSelf() {
		Instantiate (deathFX [0], transform.position, Quaternion.identity);	
		Destroy(this.gameObject);
	}

   
}

