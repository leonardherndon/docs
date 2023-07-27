using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementStackDebugViewer : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Hostile eObject;

    private void OnGUI()
    {
        if (!eObject)
        {
            eObject = GameObject.Find("bossShip").GetComponent<Hostile>();
            return;
        }

        text.text = eObject.currentBlockIndex.ToString();
    }
}
