using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class fadeUIElement : MonoBehaviour
{

    public EnergyBar energyBar;
    public bool state;
    public Text text;
    
    
    // Start is called before the first frame update
    void Start()
    {
        text.DOFade(0, 0.2f);
        state = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (energyBar.valueCurrent <= 0 && state)
        {
            Disappear();
        }
        if (energyBar.valueCurrent > 0  && !state)
        {
            Appear();
        }
    }


    public void Disappear()
    {
        text.DOFade(0, 0.2f);
        state = false;
    }

    public void Appear()
    {
        text.DOFade(1, 0.2f);
        state = true;
    }
}
