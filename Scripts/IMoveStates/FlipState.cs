using UnityEngine;

public class FlipState: IMoveState
{
    private Transform _transform;

    public FlipState(Transform transform)
    {
        _transform = transform;
    }

    public void UpdateState()
    {
        _transform.eulerAngles = _transform.eulerAngles + 180f * Vector3.up;
    }

    public void Update()
    {
        
    }
}