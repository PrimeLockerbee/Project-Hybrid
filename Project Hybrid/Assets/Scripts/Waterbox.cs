using System;
using System.Threading.Tasks;
using UnityEngine;

public class Waterbox : MovingObject
{
    [SerializeField] private float animationDuration;

    private Quaternion drainStart;
    private Quaternion flooded;
    private Quaternion drainEnd;

    private void Start()
    {
        Vector3 currentEuler = transform.rotation.eulerAngles;
        currentEuler.z = -30;
        drainStart = Quaternion.Euler(currentEuler);
        currentEuler.z = 0;
        flooded = Quaternion.Euler(currentEuler);
        currentEuler.z = 0;

    }

    public async Task Flood()
    {
        Task[] tasks = new Task[2];

        tasks[0] = MoveToInSeconds(transform.position, transform.position + Vector3.up * 1.25f, animationDuration);
        tasks[1] = RotateTowardsInSeconds(drainStart, flooded, animationDuration);

        await Task.WhenAll(tasks);
    }

    public async Task Drain()
    {
        Task[] tasks = new Task[2];

        tasks[0] = MoveToInSeconds(transform.position, transform.position - Vector3.up * 1.25f, animationDuration);
        tasks[1] = RotateTowardsInSeconds(flooded, drainStart, animationDuration);

        await Task.WhenAll(tasks);
    }
}
