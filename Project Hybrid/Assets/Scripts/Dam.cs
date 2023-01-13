using System.Threading.Tasks;
using UnityEngine;

public class Dam : MovingObject
{
    [SerializeField] private float animationDuration;
    [SerializeField] private float moveHeight;

    public async Task MoveUp()
    {
        await MoveToInSeconds(transform.position, transform.position + Vector3.up * moveHeight * GridCell.gridSize, animationDuration);
    }

    public async Task MoveDown()
    {
        await MoveToInSeconds(transform.position, transform.position - Vector3.up * moveHeight * GridCell.gridSize, animationDuration);
    }
}
