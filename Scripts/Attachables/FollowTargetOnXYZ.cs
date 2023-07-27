using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetOnXYZ : MonoBehaviour
{

    public Transform target;
    public float xPos;
    public bool followX;
    public float yPos;
    public bool followY;
    public float zPos;
    public bool followZ;



    void Start()
    {
        if (!target)
            target = GameManager.Instance.playerShip.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 altPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        if (followX)
            altPos.x = altPos.x + xPos;
        if (followY)
            altPos.y = altPos.y + yPos;
        if (followZ)
            altPos.z = altPos.z + zPos;

        transform.position = new Vector3(altPos.x, altPos.y, altPos.z);
    }
}
