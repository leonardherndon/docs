using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class BoostFX : MonoBehaviour {

    public GameObject normalMesh;
    public GameObject boostMesh;
    public GameObject systemFX;
    public GameObject cameraFX;
    //public GameObject suctionFX;
    public AudioSource boostActivationAudioSource;
    public AudioSource boostReleaseAudioSource;
    public float switchSpeed = 20f;
    public float startOffset = -140f;
    public float endOffset = -160f;
    public int delayTime = 1;
    //public bool suctionActivated = false;
    public Renderer normRend;
    public Renderer[] boostRends;
    public ParticleSystem[] boostParticles;

    public float currentOffset;

    public void Start()
    {
        normRend = normalMesh.GetComponent<Renderer>();
        boostRends = boostMesh.GetComponentsInChildren<Renderer>();
        boostParticles = boostMesh.GetComponentsInChildren<ParticleSystem>();
        boostMesh.SetActive(false);
        systemFX.SetActive(false);

        cameraFX.SetActive(false);
    }

    public IEnumerator BoostAction()
    {
        
        if(normRend == null)
        {
            normRend = normalMesh.GetComponent<Renderer>();
        }
        currentOffset = startOffset;
        boostMesh.SetActive(true);
        systemFX.SetActive(true);
        cameraFX.SetActive(true);
        foreach (Renderer boostRend in boostRends)
        {
            //Hologram
            boostRend.material.SetColor("_RimColor", ColorManager.Instance.ConvertEnumToColor(gameObject.GetComponent<ChromaShiftManager>().CurrentColor));
            boostRend.material.SetColor("_Color", ColorManager.Instance.ConvertEnumToColor(gameObject.GetComponent<ChromaShiftManager>().CurrentColor));

            //Distortion
            boostRend.material.SetColor("_InnerTint", ColorManager.Instance.ConvertEnumToColor(gameObject.GetComponent<ChromaShiftManager>().CurrentColor));
            boostRend.material.SetColor("_OuterTint", ColorManager.Instance.ConvertEnumToColor(gameObject.GetComponent<ChromaShiftManager>().CurrentColor));
        }

        foreach (ParticleSystem pSystem in boostParticles)
        {
            ParticleSystem.MainModule pMain = pSystem.main;
            pMain.startColor = ColorManager.Instance.ConvertEnumToColor(gameObject.GetComponent<ChromaShiftManager>().CurrentColor);
        }


        //Debug.Log("Absorb Action: Before ForEach Loop");
        //Preparation
        normRend.material.SetFloat("_DissolveMaskOffset", startOffset);
        normRend.material.SetFloat("_DissolveAxisInvert", -1);
        if (!GameManager.Instance.playerShip.forceBoost)
        {
            boostActivationAudioSource.Play();
        }
        //Start First Pass
        while (normRend.material.GetFloat("_DissolveMaskOffset") > endOffset + 0.5f)
        {

            currentOffset = Mathf.Lerp(currentOffset, endOffset, gameObject.GetComponent<Timeline>().deltaTime * switchSpeed);
            normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);
            //if (currentOffset <= (((endOffset + startOffset) / 2) - startOffset) && !suctionActivated)
            //{
            //    suctionFX.SetActive(true);
            //    suctionActivated = true;
            //}

            yield return null;
        }

    }

    public IEnumerator BoostDeactivate()
    {
        //First Pass Complete
        normRend.material.SetFloat("_DissolveAxisInvert", 1);
        currentOffset = startOffset;
        normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);
        boostReleaseAudioSource.Play();
        //Start Second Pass
        while (normRend.material.GetFloat("_DissolveMaskOffset") > endOffset + 0.5f)
        {
            currentOffset = Mathf.Lerp(currentOffset, endOffset, gameObject.GetComponent<Timeline>().deltaTime * switchSpeed);
            normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);

            yield return null;
        }

        //Clean Up
        normRend.material.SetFloat("_DissolveAxisInvert", -1);
        currentOffset = startOffset;
        normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);

        //suctionFX.SetActive(false);
        boostMesh.SetActive(false);
        systemFX.SetActive(false);
        cameraFX.SetActive(false);
        //suctionActivated = false;
        //Debug.Log("Absorb Action: End of Function");
    }
}
