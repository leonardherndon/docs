using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerTest : MonoBehaviour
{
    public Rigidbody rigid;

    // Update is called once per frame
    void FixedUpdate()
    {
    rigid.AddForce(500,0,0, ForceMode.Acceleration);
    
    }
}
