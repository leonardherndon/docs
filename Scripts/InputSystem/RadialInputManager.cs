using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialInputManager : UltimateRadialMenuInputManager
{
    public bool menuActive = false;
    public bool itemSelected = false;
    
    public override void CustomInput(ref bool enableMenu, ref bool disableMenu, ref Vector2 input, ref float distance, ref bool inputDown,
        ref bool inputUp, int radialMenuIndex)
    {

        Vector2 colorXY = InputSystem.Instance.GetRightStickInput();

        // Store the horizontal and vertical axis of the targeted joystick axis.
        Vector2 modInput = new Vector2( invertHorizontal ? -colorXY[0] : colorXY[0], invertVertical ? -colorXY[1] : colorXY[1] );

        // Since this is a controller, we want to make sure that our input distance feels right, so here we will temporarily store the distance before modification.
        float tempDist = Vector2.Distance( Vector2.zero, modInput );

        // Set the input to what we have calculated.
        if( modInput != Vector2.zero )
            input = modInput;

        // If the temporary distance is greater than the minimum range, then the distance doesn't matter. All we want to send to the radial menu is that it is perfectly in range, so make the distance exactly in the middle of the min and max.
        if( tempDist >= UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.minRange )
            distance = Mathf.Lerp( UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMinRange, UltimateRadialMenuInformations[ radialMenuIndex ].radialMenu.CalculatedMaxRange, 0.5f );


        if (!itemSelected)
        {
            if (GameManager.Instance.playerController.GetAxis("ColorX") > 0.65f ||
                GameManager.Instance.playerController.GetAxis("ColorX") < -0.65f ||
                GameManager.Instance.playerController.GetAxis("ColorY") > 0.65f ||
                GameManager.Instance.playerController.GetAxis("ColorY") < -0.65f)
            {
                inputDown = true;
                Debug.Log("Input Down.");
                itemSelected = true;
            }
        }
        else
        {
            itemSelected = false;
        }

        /*if (itemSelected)
        {
            if (GameManager.Instance.playerController.GetAxis("ColorX") < 0.65f &&
                GameManager.Instance.playerController.GetAxis("ColorX") > -0.65f &&
                GameManager.Instance.playerController.GetAxis("ColorY") < 0.65f &&
                GameManager.Instance.playerController.GetAxis("ColorY") > -0.65f)
            {
                inputUp = true;
                Debug.Log("Input Up.");
                itemSelected = false;
            }
        }*/


        if (!menuActive)
        {
            if (GameManager.Instance.playerController.GetAxis("ColorX") > 0.1f ||
                GameManager.Instance.playerController.GetAxis("ColorX") < -0.1f ||
                GameManager.Instance.playerController.GetAxis("ColorY") > 0.1f ||
                GameManager.Instance.playerController.GetAxis("ColorY") < -0.1f)
            {
                Debug.Log("Menu Enabled.");
                enableMenu = true;
                menuActive = true;
                itemSelected = false;
            }
            
        }

        if (menuActive)
        {
            if (GameManager.Instance.playerController.GetAxis("ColorX") < 0.05f &&
                GameManager.Instance.playerController.GetAxis("ColorX") > -0.05f &&
                GameManager.Instance.playerController.GetAxis("ColorY") < 0.05f &&
                GameManager.Instance.playerController.GetAxis("ColorY") > -0.05f)
            {
                Debug.Log("Menu Disabled.");
                inputDown = true;
                disableMenu = true;
                menuActive = false;
                itemSelected = false;
            }
        }
    }
}
