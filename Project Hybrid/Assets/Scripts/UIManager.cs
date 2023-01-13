using MarcoHelpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject menu;
    public Text percentageText;
    public Text funFactText;

    private GridManager gridManager;

    void Start()
    {
        gridManager = ServiceLocator.GetService<GridManager>();
        HideEndScreen();
    }

    private void OnEnable()
    {
        EventSystem.Subscribe(EventName.FUEL_EMPTY, (value) => ShowEndScreen());
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe(EventName.FUEL_EMPTY, (value) => ShowEndScreen());
    }

    public void ShowEndScreen()
    {
        menu.SetActive(true);
        percentageText.text = $"You have removed {gridManager.CalculateCleanPercent()}% of garbage from the ocean";
    }

    public void HideEndScreen()
    {
        menu.SetActive(false);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
