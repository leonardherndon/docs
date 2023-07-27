using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipPassthrough : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerShip playerShip;

    public void FinishFlip()
    {
        playerShip.GearMovementApp.FinishFlip();
    }

    public void ExitBarrelRoll()
    {
        playerShip.GearMovementApp.ExitBarrelRoll();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
