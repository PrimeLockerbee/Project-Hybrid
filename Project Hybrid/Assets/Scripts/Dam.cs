using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Dam : MovingObject
{
    [SerializeField] private float animationDuration;
    [SerializeField] private float moveHeight;

    [SerializeField] private List<Gear> damGears;

    public async Task MoveUp()
    {
        if (damGears != null)
        {
            foreach (Gear gear in damGears)
            {
                gear.RotateOpen(animationDuration);
            }
        }

        await MoveToInSeconds(transform.position, transform.position + Vector3.up * moveHeight * GridCell.gridSize, animationDuration);
    }

    public async Task MoveDown()
    {
        if (damGears != null)
        {
            foreach (Gear gear in damGears)
            {
                gear.RotateClosed(animationDuration);
            }
        }
        await MoveToInSeconds(transform.position, transform.position - Vector3.up * moveHeight * GridCell.gridSize, animationDuration);
    }
}
