using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackOnTrigger : MonoBehaviour
{

    [SerializeField] private MMFeedbacks feedback;
    public MMFeedbacks Feedback
    {
        get => feedback;
        set => feedback = value;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        feedback.PlayFeedbacks();
    }
}
