using UnityEngine;
using UnityEngine.EventSystems;

public class FollowTarget : MovingObject
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateTime;
    private Vector3 offsetToTarget;

    private void Start()
    {
        offsetToTarget = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform.position = target.position + offsetToTarget;
    }

    public void RotateWithPlayer(Direction _moveDirection, float _timeModifier)
    {
        RotateWithSlerpInSeconds(transform.rotation, _moveDirection.GetRotation(), rotateTime * _timeModifier);
    }
}
