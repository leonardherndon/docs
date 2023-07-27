using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;

public class VolumetricLaser : MonoBehaviour//, ILaser
{
    public GameObject HitEffect;
    public float HitOffset = 0;
    private CollisionController CoC;
    [SerializeField]
    private HostileType hostileType;
    public Transform laserOrigin;
    [SerializeField]
    private float MaxLength;
    [SerializeField]
    private float BlastSpeed;
    [SerializeField]
    private float BlastShrink;
    private LineRenderer Laser;
    [SerializeField]
    private float CurrentLaserLength;
    [SerializeField]
    private float CurrentLaserWidth;
    public float cachedWidth;

    public float MainTextureLength = 1f;
    public float NoiseTextureLength = 1f;
    private Vector4 Length = new Vector4(1, 1, 1, 1);
    private Vector4 LaserSpeed = new Vector4(0, 0, 0, 0);
    private Vector4 LaserStartSpeed;
    //One activation per shoot
    private bool LaserSaver = false;
    private bool UpdateSaver = false;

    public LaserType laserType;

    //void Start()
    //{
    //    if(!RCM)
    //    {
    //        RCM = gameObject.AddComponent<ProjectileCollisionManager>();
    //    }

    //    StartCoroutine(LaserBlast());
    //}

    //public void LaserHit ()
    //{
    //   //DOES NOTHING YET
    //}

    //public IEnumerator LaserBlast()
    //{
    //    // DOES NOTHING YET
    //}

    public void DisablePrepare()
    {
        Destroy(this.gameObject);
    }
}
