using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBumper : MonoBehaviour
{
    public bool isTopBumper;
    public AudioSource _audioSource;
    [SerializeField] private float bumperDamage = 100000f;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Scene"))
        {
            return;
        }
        
        if (isTopBumper)
        {
            GameManager.Instance.playerShip.isBumperTop = true;
        }
        else
        {
            GameManager.Instance.playerShip.isBumperBottom = true;
        }

        GameManager.Instance.playerShip.isBumperCollided = true;
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Scene"))
        {
            return;
        }
        
        if (isTopBumper)
        {
            GameManager.Instance.playerShip.isBumperTop = true;
        }
        else
        {
            GameManager.Instance.playerShip.isBumperBottom = true;
        }

        GameManager.Instance.playerShip.isBumperCollided = true;
        if (GameManager.Instance.playerShip.isBumperTop && GameManager.Instance.playerShip.isBumperBottom)
        {
            GameManager.Instance.playerShip.LS.DrainLife(bumperDamage, ImpactType.Physical);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Scene"))
        {
            return;
        }
        
        if (isTopBumper)
        {
            GameManager.Instance.playerShip.GearMovementApp.ShipMoveDown = false;
            GameManager.Instance.playerShip.isBumperTop = false;
        }
        else
        {
            GameManager.Instance.playerShip.GearMovementApp.ShipMoveUp = false;
            GameManager.Instance.playerShip.isBumperBottom = false;
        }

        GameManager.Instance.playerShip.isBumperCollided = false;
    }
}