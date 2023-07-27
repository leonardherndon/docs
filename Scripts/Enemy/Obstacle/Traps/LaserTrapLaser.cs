using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using ChromaShift.Scripts;
using ChromaShift.Scripts.Enemy;
using Chronos;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class LaserTrapLaser : MonoBehaviour, ILaser
{
    [FormerlySerializedAs("MaxLength")] [SerializeField]
    private float LaserLength;
    [SerializeField]
    private float BlastSpeed;
    private LineRenderer Laser;
    [SerializeField]
    private float LaserWidth;
    [SerializeField]
    private float laserDelay;
    [SerializeField]
    private float laserDelayMid;
    [SerializeField]
    private float laserDelayEnd;

    [SerializeField] private BoxCollider collider;

    private void Start()
    {
        collider.enabled = false;
        Laser = GetComponent<LineRenderer>();
        Laser.enabled = true;

        Vector3 EndPos = transform.position + transform.forward * LaserLength;
        Laser.SetPosition(0, transform.position);
        Laser.SetPosition(1, EndPos);

        StartCoroutine(LaserBlast());
    }
    
    public IEnumerator LaserBlast()
    {
        yield return new WaitForSeconds(laserDelay);
        DOTween.To(()=> Laser.startWidth, x=> Laser.startWidth = x, LaserWidth, BlastSpeed);
        DOTween.To(()=> Laser.endWidth, x=> Laser.endWidth = x, LaserWidth, BlastSpeed);
        collider.enabled = true;
        yield return new WaitForSeconds(laserDelayMid);
        DOTween.To(()=> Laser.startWidth, x=> Laser.startWidth = x, 0, laserDelayEnd);
        DOTween.To(()=> Laser.endWidth, x=> Laser.endWidth = x, 0, laserDelayEnd);
        collider.enabled = false;
        yield return new WaitForSeconds(laserDelayEnd);
        DisablePrepare();
    }

    public void DisablePrepare()
    {
        if (Laser != null)
        {
            Laser.enabled = false;
        }

        Destroy(this.gameObject);
    }
}
