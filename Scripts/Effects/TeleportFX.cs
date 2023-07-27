using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class TeleportFX : MonoBehaviour
{

    public Renderer shipRenderer;
	public Material defaultMaterial;
    public Material teleportMaterial;
    public GameObject dropCloneObject;
    public AudioSource activationAudioSource;
    public MMFeedbacks mmfTeleportActivate;
    public float fadeSpeed =  0.4f;
    public float transitionSpeed = 2f;
    public float camResetSpeed = 2f;
    public float startOffset  = 0f;
    public float endOffset = 1;
    public GameObject teleportFXObject;
    public GameObject teleportTrail;
    private GameObject liveTrail;
    public float currentOffset;
    public bool teleportFXStart;
    public bool teleportFXEnd;

    public void Start()
    {
        //TODO FIX TELEPORT FX MATERIALS
        //shipRenderer = GetComponent<Renderer>();
        //defaultMaterial = shipRenderer.GetComponent<MeshRenderer>().materials[1];
        //teleportMaterial = shipRenderer.GetComponent<MeshRenderer>().materials[0];
        //teleportParticles = teleportFXObject.GetComponentsInChildren<ParticleSystem>();
        
    }

    public IEnumerator TeleportAction()
    {
        teleportFXStart = false;
        currentOffset = startOffset;
        //activationAudioSource.Play();
        mmfTeleportActivate.PlayFeedbacks();
        //teleportTrail.SetActive(true);
        
        //Spawn Drop Clone
        var dropClone = Instantiate(dropCloneObject, transform.position, Quaternion.identity);
        //dropClone.transform.parent = teleportFXObject.transform;
        Destroy(liveTrail);
        liveTrail = Instantiate(teleportTrail, transform.position, Quaternion.identity);
        liveTrail.transform.parent = teleportFXObject.transform;
        
        //Loosen Camera
        GameManager.Instance.mainCamera.HorizontalFollowSmoothness = 0.65f;
         
        
        //Start First Pass
        defaultMaterial.DOFade(0, fadeSpeed);
        
        //First Pass Complete
        currentOffset = endOffset;
        teleportMaterial.SetFloat("_Stealth", currentOffset);
        yield return null;
    }
    
    public IEnumerator TeleportComplete()
    {
        Destroy(liveTrail);
        
        //Start Second Pass
        defaultMaterial.DOFade(1, fadeSpeed);
        
        while (teleportMaterial.GetFloat("_Stealth") > startOffset + 0.1f || GameManager.Instance.mainCamera.HorizontalFollowSmoothness > 0.16f)
        {
            currentOffset = Mathf.Lerp(currentOffset, startOffset, Timekeeper.instance.Clock("Player").fixedDeltaTime * transitionSpeed);
            teleportMaterial.SetFloat("_Stealth", currentOffset);
            GameManager.Instance.mainCamera.HorizontalFollowSmoothness =
                Mathf.Lerp(GameManager.Instance.mainCamera.HorizontalFollowSmoothness, 0.15f,
                    camResetSpeed * Timekeeper.instance.Clock("Player").fixedDeltaTime);
            yield return null;
        }
        
        //Second Pass Complete
        //teleportTrail.SetActive(false);
        
        currentOffset = startOffset;
        teleportMaterial.SetFloat("_Stealth", currentOffset);
        teleportFXEnd = true;
        
        GameManager.Instance.mainCamera.HorizontalFollowSmoothness = 0.15f;
    }


}
