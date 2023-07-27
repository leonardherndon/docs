using UnityEngine;
using System.Collections;
using DG.Tweening;
using Chronos;

public class PickupAbsorbMovement : MonoBehaviour
{
    public Quaternion rotateThis;
    private Transform target;
    private Vector3 step;
    float vectorDistance;
    public float speed = 50f;
    public float absorbRange = 10f;

    void Start()
    {
        if (!target)
        {
            target = GameManager.Instance.playerShip.transform;
        }
        
        step = Vector3.zero;
    }

    public void FixedUpdate()
    {
        if(GameManager.Instance.playerShip.isShieldActive)
            MoveObject();
    }

    private void MoveObject()
    {

        Vector3 shipVelocity;
        //SHIP SHOULD ALWAYS BE MOVING FORWARD
        shipVelocity = new Vector3(1, 0, 0).normalized;

        vectorDistance = Vector3.Distance(transform.position, target.transform.position);
        if(vectorDistance > absorbRange)
        {
            return;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), Timekeeper.instance.Clock("Pickup").fixedDeltaTime * 100);

        step = transform.forward * Timekeeper.instance.Clock("Pickup").fixedDeltaTime * speed;

        // Advances missile forward
        transform.position += step;


    }

}
