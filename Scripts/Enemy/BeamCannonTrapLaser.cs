using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using Chronos;
using UnityEngine;
using DG.Tweening;
public class BeamCannonTrapLaser : MonoBehaviour, ILaser
{
    public GameObject HitEffect;
    public float HitOffset = 0;
    private CollisionController CoC;
    [SerializeField]
    private HostileType hostileType;
    public GameObject[] laserNodes;
    [SerializeField]
    private float hitDelay;
    [SerializeField]
    private float maxWidth;
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
    [SerializeField]
    private float verticalDistance;
    [SerializeField]
    private float horizontalDistance;
    [SerializeField]
    private float depthDistance;
    [SerializeField]
    private float LaserLength;
    //One activation per shoot
    private bool LaserSaver = false;
    private bool UpdateSaver = false;
    private Clock _clock;

    public LaserType laserType;

    void Awake()
    {
        laserNodes = new GameObject[2];
    }
    void Start()
    {

        _clock = Timekeeper.instance.Clock("Enemy");
        
        laserNodes[0] = new GameObject("Laser Node " + 0);
        laserNodes[1] = new GameObject("Laser Node " + 1);
//        laserNodes[0].transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
//        laserNodes[1].transform.parent = GameManager.Instance.spawnedObjectsHolder.transform;
        
        //Get LineRender and ParticleSystem components from current prefab;  
        Laser = GetComponent<LineRenderer>();
        Laser.enabled = true;
        if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) LaserStartSpeed = Laser.material.GetVector("_SpeedMainTexUVNoiseZW");
        //Save [1] and [3] textures speed
        LaserSpeed = LaserStartSpeed;
        CurrentLaserWidth = Laser.startWidth;
        cachedWidth = Laser.startWidth;

        
        
        

       
        StartCoroutine(LaserBlast());
    }

    public void LaserHit()
    {
    //    Debug.Log("Entering LaserHit()");
    //    RaycastHit[] hits;

        
    //    Debug.DrawRay(laserOriginPos, transform.right, Color.green);
    //    Debug.Log("After DrawRay | " + laserOriginPos);
    //    foreach (RaycastHit hit in hits)
    //    {

    //        Debug.Log("Raycast Hit:" + hit.transform.name);
    //        if (hit.transform.CompareTag("Player"))
    //        {

    //            //Create Hit Effect Here
    //            GameObject HitMarker = Instantiate(HitEffect, hit.transform.position, Quaternion.identity);
    //            //HitEffect.transform.position = hit.point + hit.normal * HitOffset;
    //            HitMarker.transform.parent = GameManager.Instance.playerShip.transform;
    //            RCM.ManageHostileHitOnPlayer(transform);

    //        }
    //    }

    }

    public IEnumerator LaserBlast()
    {
        Debug.Log("BeforeBeamDelay");
        yield return new WaitForSeconds(hitDelay);

        laserNodes[0].transform.position = new Vector3(Camera.main.transform.position.x + horizontalDistance, Camera.main.transform.position.y + verticalDistance, depthDistance);
        laserNodes[1].transform.position = new Vector3(Camera.main.transform.position.x - horizontalDistance, Camera.main.transform.position.y - verticalDistance, depthDistance);

        LaserLength = Vector3.Distance(laserNodes[0].transform.position, laserNodes[1].transform.position);

        Laser.SetPosition(0, laserNodes[0].transform.position);

        Vector3 relativePos = new Vector3(Camera.main.transform.position.x - horizontalDistance, Camera.main.transform.position.y - verticalDistance, depthDistance) - laserNodes[0].transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.rotation = rotation;
        Debug.Log("AfterBeamDelay");

        //Laser.SetPosition(1, new Vector3(Camera.main.transform.position.x - horizontalDistance, Camera.main.transform.position.y - verticalDistance, depthDistance));
        //Reset Laser width:
        Laser.startWidth = cachedWidth;
        Laser.endWidth = cachedWidth;
        CurrentLaserLength = 0;
        CurrentLaserWidth = cachedWidth;
        //LaserHit();

        while (CurrentLaserLength < LaserLength - 0.5)
        {
            Debug.Log("Current Laser Length:" + CurrentLaserLength);
            CurrentLaserLength = Mathf.Lerp(CurrentLaserLength, LaserLength, (_clock.deltaTime * BlastSpeed));
            var EndPos = laserNodes[0].transform.position + transform.forward * CurrentLaserLength;
            //Debug.Log("Current Laser Length: " + CurrentLaserLength + "\n Current EndPos: "+EndPos);
            Laser.SetPosition(1, EndPos);
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        GameManager.Instance.mainCamera.GameCamera.DOShakePosition(5f, 15f, 10, 45, true);
        GameManager.Instance.playerShip.CoC.GetWrecked();
        

        while (CurrentLaserWidth < 35 - 0.05f)
        {
            //Debug.Log("Current Laser Width:" + CurrentLaserWidth);
            CurrentLaserWidth = Mathf.Lerp(CurrentLaserWidth, 35, (_clock.deltaTime * BlastShrink));
            Laser.startWidth = CurrentLaserWidth;
            Laser.endWidth = CurrentLaserWidth;
            yield return null;
        }

        //Reset Laser length
        //Laser.SetPosition(1, laserOrigin.position);
        //DisablePrepare();
    }

    public void DisablePrepare()
    {
        if (Laser != null)
        {
            Laser.enabled = false;
        }
        UpdateSaver = true;

        Destroy(this.gameObject);
    }
}
