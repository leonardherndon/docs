using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindBumper : MonoBehaviour
{
    
    public AudioSource _audioSource;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Scene"))
        {
            return;
        }
        
        GameManager.Instance.playerShip.isGrinding = true;
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Scene"))
        {
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Instance.playerShip.isGrinding = false;
    }
}
