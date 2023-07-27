using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using EnergyBarToolkit;

public class SuccessBarColorShift : MonoBehaviour {

    private Color[] colors = new Color[8];

    public int currentIndex = 0;
    private int nextIndex;

    public float changeColourTime = 2.0f;

    private float lastChange = 0.0f;
    private float timer = 0.0f;

    void Start()
    {
        colors[0] = ColorManager.Instance.colorArray[0];
        colors[1] = ColorManager.Instance.colorArray[1];
        colors[2] = ColorManager.Instance.colorArray[2];
        colors[3] = ColorManager.Instance.colorArray[3];
        colors[4] = ColorManager.Instance.colorArray[4];
        colors[5] = ColorManager.Instance.colorArray[5];
        colors[6] = ColorManager.Instance.colorArray[9];
        colors[7] = ColorManager.Instance.colorArray[10];

        if (colors == null || colors.Length < 2)
            Debug.Log("Need to setup colors array in inspector");

        nextIndex = (currentIndex + 1) % colors.Length;
    }

    void FixedUpdate()
    {
        //if(GameStateManager.Instance.CurrentState.StateType != GameState.Loading)
        //{
        //    return;
        //}

        timer += Time.deltaTime;

        if (timer > changeColourTime)
        {
            currentIndex = (currentIndex + 1) % colors.Length;
            nextIndex = (currentIndex + 1) % colors.Length;
            timer = 0.0f;

        }
        GetComponent<FilledRendererUGUI>().spriteBarColor = Color.Lerp(colors[currentIndex], colors[nextIndex], timer / changeColourTime);
        //GetComponent<Image>().material.color = Color.Lerp(colors[currentIndex], colors[nextIndex], timer / changeColourTime);
    }
}