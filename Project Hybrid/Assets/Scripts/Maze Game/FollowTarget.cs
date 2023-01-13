using UnityEngine;

public class FollowTarget : MovingObject
{
    [SerializeField] private Transform target;
    private Vector3 offsetToTarget;

    private void Start()
    {
        offsetToTarget = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform.position = target.position + offsetToTarget;
    }
}
