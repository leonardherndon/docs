using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CS_Audio;

public class MagMine : Hostile, IKillable {

    public MovableObjectController mObject;
    public AudioSource aS;
    public float magFactor;

    protected void Start()
    {
        base.Start();
        aS = gameObject.GetComponent<AudioSource>();
        mObject = gameObject.GetComponent<Hostile>().MoC;
        //TODO REMOVE FROM HARDCODING THE TARGET
        targetObject = GameManager.Instance.playerShip.gameObject;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        magFactor = mObject.magFactor;
        if (magFactor > 0 && !aS.isPlaying)
        {
            aS.loop = true;
            aS.Play();
        }
        if (magFactor <= 0 && aS.isPlaying)
        {
            aS.Stop();
        }

    }

    override public void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        AudioManager.Instance.PlayClipWrap();
    }
}
