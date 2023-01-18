using UnityEngine;

public class FollowTarget : MovingObject
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotateTime;
    [SerializeField] private float raycastInterval = 0.4f;
    private Vector3 offsetToTarget;

    private void Start()
    {
        offsetToTarget = transform.position - target.position;
        InvokeRepeating(nameof(CustomUpdate), 0.01f, raycastInterval);
    }

    private void CustomUpdate()
    {
        //Debug.Log("CUSTOM");
        Vector3 directionToTarget = target.position - transform.position;
        float distanceToPlayer = directionToTarget.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget.normalized, out hit, distanceToPlayer, LayerMask.NameToLayer("CameraHide")))
        {
            Debug.Log("Hit!");
            HideFromCamera hideObject = hit.collider.gameObject.GetComponent<HideFromCamera>();
            if (hideObject != null && !hideObject.isHidden)
            {
                Debug.Log("Hide");
                hideObject.Hide();
            }
        }
    }

    private void LateUpdate()
    {
        transform.position = target.position + offsetToTarget;
    }

    public void RotateWithPlayer(Direction _moveDirection, float _timeModifier)
    {
        RotateWithSlerpInSeconds(transform.rotation, _moveDirection.GetRotation(), rotateTime * _timeModifier);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, target.position - transform.position);
    }
}
