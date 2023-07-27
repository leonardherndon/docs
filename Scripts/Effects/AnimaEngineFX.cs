using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using DG.Tweening;
using EnergyBarToolkit;

public class AnimaEngineFX : MonoBehaviour
{

    public GameObject normalMeshObject;
    public GameObject animaCloneObject;
    public AudioSource activationAudioSource;
    public GameObject ui;
    
    public void Start()
    {
        if (!normalMeshObject)
        {
            Debug.LogError("Normal Mesh Not Selected. Please Select one");
        }
        
        if (!animaCloneObject)
        {
            Debug.LogError("Anima Object Not Selected. Please Select one");
        }
        
    }

    public void StartAnima()
    {
        activationAudioSource.Play();
        animaCloneObject.SetActive(true);
        normalMeshObject.SetActive(false);
    }
    
    public void EndAnima()
    {
        GameManager.Instance.mainCamera.OffsetX = 0;
        animaCloneObject.SetActive(false);
        normalMeshObject.SetActive(true);
    }
    
}
