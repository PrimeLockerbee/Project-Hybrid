using UnityEngine;

public class Gear : MovingObject
{
    public int degrees;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.AngleAxis(degrees, Vector3.forward);
        //openRotation = Quaternion.Euler(closedRotation.eulerAngles.x, closedRotation.eulerAngles.y, closedRotation.eulerAngles.z + degrees);

    }

    public void RotateOpen(float _timeInSeconds)
    {
        RotateTowardsInSeconds(closedRotation, openRotation, _timeInSeconds);
    }

    public void RotateClosed(float _timeInSeconds)
    {
        RotateTowardsInSeconds(openRotation, closedRotation, _timeInSeconds);
    }
}
