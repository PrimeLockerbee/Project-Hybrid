using System.Threading.Tasks;
using UnityEngine;

public class Dam : MovingObject
{
    [SerializeField] private float animationDuration;
    [SerializeField] private float moveHeight;

    public async void MoveUp()
    {
        await MoveToInSeconds(transform.position, transform.position + Vector3.up * moveHeight, animationDuration);
    }

    public async void MoveDown()
    {
        await MoveToInSeconds(transform.position, transform.position - Vector3.up * moveHeight, animationDuration);
    }
}
