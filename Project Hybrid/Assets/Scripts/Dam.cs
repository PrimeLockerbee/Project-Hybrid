using System.Threading.Tasks;
using UnityEngine;

public class Dam : MovingObject
{
    [SerializeField] private float animationDuration;

    private bool isOpen;

    private void Update()
    {
        CheckInput();
    }

    private async void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isOpen)
            {
                await MoveToInSeconds(transform.position, transform.position + Vector3.up * 3, animationDuration);
                FindObjectOfType<Waterbox>().Flood();
                Debug.Log("Open");
            }
            else
            {
                await MoveToInSeconds(transform.position, transform.position - Vector3.up * 3, animationDuration);
                FindObjectOfType<Waterbox>().Drain();
                Debug.Log("Close");
            }

            isOpen = !isOpen;
        }
    }

}
