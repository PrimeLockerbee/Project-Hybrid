using System.Threading.Tasks;
using UnityEngine;

public class Dam : MovingObject
{
    [SerializeField] private float animationDuration;
    [SerializeField] private float moveHeight;

    public async Task MoveUp()
    {
        /*Debug.Log("Stap 3");
        
        transform.position = Vector3.up;
        Debug.Log("Stap 3.5");*/


        Debug.Log("Stap 3");
        await MoveToInSeconds(transform.position, transform.position + Vector3.up * moveHeight * GridCell.gridSize, animationDuration);
    }

    public async Task MoveDown()
    {
        /*Debug.Log("Stap 3");
        transform.position = Vector3.up;

        Debug.Log("Stap 3.5");*/

        Debug.Log("Stap 3");
        await MoveToInSeconds(transform.position, transform.position - Vector3.up * moveHeight * GridCell.gridSize, animationDuration);
    }
}
