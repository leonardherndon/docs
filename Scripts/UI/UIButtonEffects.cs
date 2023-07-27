using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonEffects : MonoBehaviour
{

    public Text textBlock;
    // Use this for initialization
    void Start()
    {
        textBlock = gameObject.GetComponentInChildren<Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Enter");
        textBlock.color = ColorManager.Instance.colorArray[10];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exit");
        textBlock.color = ColorManager.Instance.colorArray[6];
    }
}
