using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FlyCamera : MovingObject
{
    public AnimationCurve movementCurve;
    public float animationDuration = 5f;
    public Vector3 offsetFromBoat = Vector3.up;
    public float finalXAngle = 85;

    private Quaternion endRotation;

    private void Awake()
    {
        endRotation = Quaternion.Euler(finalXAngle, 0, 0);
    }

    public async Task Play(Vector3 _position)
    {
        Debug.Log(_position + offsetFromBoat);
        RotateTowardsInSeconds(transform.rotation, endRotation, animationDuration);
        RenderSettings.fogDensity += 0.01f * Time.deltaTime;
        await MoveToInSecondsWithCurve(transform.position, _position + offsetFromBoat, animationDuration);
    }

    /// <summary>
    /// Moves the current object from oldPos to a new position within a given period of time.
    /// </summary>
    /// <param name="oldPos"></param>
    /// <param name="targetPos"></param>
    /// <param name="timeInSeconds"></param>
    public async Task MoveToInSecondsWithCurve(Vector3 oldPos, Vector3 targetPos, float timeInSeconds)
    {
        transform.position = oldPos;
        float distance = Vector3.Distance(oldPos, targetPos);
        float moveSpeed = distance / timeInSeconds;
        float t = 0;

        while (t < timeInSeconds)
        {
            //transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(oldPos, targetPos, movementCurve.Evaluate(t/timeInSeconds));
            t += Time.deltaTime;
            await Task.Yield();
        }
        transform.position = targetPos;

        //onDone?.Invoke();
    }
}
