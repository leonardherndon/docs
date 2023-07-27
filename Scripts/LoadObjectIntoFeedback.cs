using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class LoadObjectIntoFeedback : MonoBehaviour
{
    [SerializeField] private string _targetObjectName;
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private Image[] searchQueue; 
    [SerializeField] private MMFeedbackSetActive _feedbacks;
    // Start is called before the first frame update
    void Start()
    {
        _feedbacks = GetComponent<MMFeedbackSetActive>();
        HuntForObject();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_feedbacks.TargetGameObject == null)
        {
            HuntForObject();
        }
    }

    void HuntForObject()
    {
        searchQueue = FindObjectsOfType<Image>();
        foreach (Image imageObject in searchQueue)
        {
            if (imageObject.gameObject.name == _targetObjectName)
            {
                _targetObject = imageObject.gameObject;
                _feedbacks.TargetGameObject = _targetObject;
                return;
            }
        }
        
        Debug.LogWarning("No Object Found for search Queue.");
    }
}
