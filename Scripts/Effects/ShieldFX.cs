using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using UnityEngine.Serialization;

public class ShieldFX : MonoBehaviour {

	public GameObject normalMesh;
    [FormerlySerializedAs("shieldMesh")] [FormerlySerializedAs("absorbMesh")] public GameObject effectMesh;
    public GameObject suctionFX;
    public GameObject systemFX;
    [FormerlySerializedAs("AbsorbActivationAudioSource")] public AudioSource effectActivationAudioSource;
    public AudioSource effectReverseAudioSource;
    public float switchSpeed = 2f;
    public float startOffset  = -143f;
    public float endOffset = -154f;
    public bool suctionActivated = false;
    public Renderer normRend;
    [FormerlySerializedAs("absorbRends")] public Renderer[] effectRends;
    [FormerlySerializedAs("absorbParticles")] public ParticleSystem[] effectParticles;
    public float currentOffset;

    public void Start()
    {
        normRend = normalMesh.GetComponent<Renderer>();
        effectRends = effectMesh.GetComponentsInChildren<Renderer>();
        effectParticles = effectMesh.GetComponentsInChildren<ParticleSystem>();
        effectMesh.SetActive(false);
        systemFX.SetActive(false);
    }

    public IEnumerator ShieldAction()
    {
        currentOffset = startOffset;
        effectMesh.SetActive(true);
        systemFX.SetActive(true);
        
        //Debug.Log("Absorb Action: Before ForEach Loop");
        //Preparation
        normRend.material.SetFloat("_DissolveMaskOffset", startOffset);
        normRend.material.SetFloat("_DissolveAxisInvert", -1);
        effectActivationAudioSource.Play();

        //Start First Pass
        while (normRend.material.GetFloat("_DissolveMaskOffset") > endOffset + 0.5f)
        {

            currentOffset = Mathf.Lerp(currentOffset, endOffset,
                gameObject.GetComponent<Timeline>().deltaTime * switchSpeed);
            normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);
            if (suctionFX != null)
            {
                if (currentOffset <= (((endOffset + startOffset) / 2) - startOffset) && !suctionActivated)
                {
                    suctionFX.SetActive(true);
                    suctionActivated = true;
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(1);

        
        
        yield return null;

    }

    public IEnumerator ShieldShutdown() {
        //Clean Up
        //First Pass Complete
        normRend.material.SetFloat("_DissolveAxisInvert", 1);
        currentOffset = startOffset;
        normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);
        effectReverseAudioSource.Play();
        //Start Second Pass
        while (normRend.material.GetFloat("_DissolveMaskOffset") > endOffset + 0.5f)
        {
            currentOffset = Mathf.Lerp(currentOffset, endOffset,
                Timekeeper.instance.Clock("Player").fixedDeltaTime * switchSpeed);
            normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);

            yield return null;
        }
        
        normRend.material.SetFloat("_DissolveAxisInvert", -1);
        currentOffset = startOffset;
        normRend.material.SetFloat("_DissolveMaskOffset", currentOffset);

        if (suctionFX != null)
        {
            suctionFX.SetActive(false);
        }

        systemFX.SetActive(false);
        effectMesh.SetActive(false);
        if (suctionFX != null)
        {
            suctionActivated = false;
        }

//        Debug.Log("Absorb Action: End of Function");
        yield return null;
    }


}
