using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrap<GameObject,Collider> {
    GameObject TargetEffect {get; set;}
    GameObject Payload { get; set; }

    void OnTriggerEnter(Collider collider);

    void Spawn();

    IEnumerator LaunchProjectile();

    void DisposeSelf();
}

