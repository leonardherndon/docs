using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using DG.Tweening;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [SerializeField] private float min = 0;

    [SerializeField] private float max = 180f;
    [SerializeField] private float duration = 0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalRotate(new Vector3(transform.rotation.x, transform.rotation.y, Random.Range(min, max)), duration);
    }

}
