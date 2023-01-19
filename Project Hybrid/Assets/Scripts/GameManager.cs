using UnityEngine;

public class GameManager : MonoBehaviour
{
    private FlyCamera flyCamera;
    private Camera boatCamera;
    private Boat boat;

    private void Awake()
    {
        flyCamera = FindObjectOfType<FlyCamera>();
    }

    private async void Start()
    {
        // Do Camera Stuff
        boatCamera = FindObjectOfType<FollowTarget>().GetComponentInChildren<Camera>();
        boat = FindObjectOfType<Boat>();
        boatCamera.gameObject.SetActive(false);
        await flyCamera.Play(boat.transform.position);

        // Start Boat
        flyCamera.gameObject.SetActive(false);
        boatCamera.gameObject.SetActive(true);
        boat.OnStart();
        RenderSettings.fogDensity = 0.005f;
    }
}
