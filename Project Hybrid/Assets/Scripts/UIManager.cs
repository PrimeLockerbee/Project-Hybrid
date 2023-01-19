using MarcoHelpers;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menu;
    [SerializeField] private Text percentageText;
    [SerializeField] private Text funFactText;

    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject fuelbar;

    [Header("Fun facts")]
    [SerializeField] private List<FunFact> funFacts;

    private GridManager gridManager;

    private void Awake()
    {
        ServiceLocator.RegisterService(this);
    }

    private void Start()
    {
        gridManager = ServiceLocator.GetService<GridManager>();
        HideEndScreen();
        fuelbar.gameObject.SetActive(false);
        Invoke(nameof(ShowFuelBar), 5f);
        //ShowEndScreen();
    }

    private void ShowFuelBar()
    {
        fuelbar.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ReloadScene();
        }
    }

    /*    private void OnEnable()
        {
            EventSystem.Subscribe(EventName.FUEL_EMPTY, (value) => ShowEndScreen());
        }

        private void OnDisable()
        {
            EventSystem.Unsubscribe(EventName.FUEL_EMPTY, (value) => ShowEndScreen());
        }*/

    public void ShowEndScreen()
    {
        if (minimap != null) minimap.SetActive(false);
        if (fuelbar != null) fuelbar.SetActive(false);

        if (menu != null) menu.SetActive(true);

        int cleanPercentage = gridManager.CalculateCleanPercent();
        percentageText.text = $"You have removed {cleanPercentage}% of garbage from the ocean";
        funFactText.text = GetFunFactText(cleanPercentage);
    }

    public void HideEndScreen()
    {
        menu.SetActive(false);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string GetFunFactText(int _cleanPercentage)
    {
        List<FunFact> viableFunFacts = funFacts.Where(fact => fact.isPercentageInRange(_cleanPercentage)).ToList();

        if (viableFunFacts.Count == 0) return $"You have removed {gridManager.CalculateCleanPercent()}% of garbage from the ocean";
        else 
        {
            string factText = viableFunFacts[Random.Range(0, viableFunFacts.Count)].text;
            return factText.Replace("PERCENTAGE", _cleanPercentage + "%");
        }
    }
}
