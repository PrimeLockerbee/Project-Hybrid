using UnityEngine;
using MarcoHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class Boat : MovingObject
{
    [SerializeField] private StatBar fuelBar;


    [Tooltip("The lower the number, the more the ship will try to keep straight.")]
    [Range(-10, 0)]
    [SerializeField] private float SameDirectionScoreModifier = -1;
    [Tooltip("With higher numbers, the ship is less likely to turn around")]
    [Range(0, 10)]
    [SerializeField] private float OppositeDirectionScoreModifier = 1;

    [SerializeField] private float maxFuel = 100;

    private float fuel;
    private GridManager _gridManager;
    private Direction lastDirection;

    private void Awake()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    private void Start()
    {
        fuel = maxFuel / 2;     // Start with half of the max fuel

        CleanCurrentTile();
        MoveToLowestLevel();
    }

    public void AddFuel(float _amount)
    {
        fuel += _amount;
        fuel = Mathf.Clamp(fuel, 0, maxFuel);

        if (fuelBar != null)
        {
            fuelBar.SetSliderValue(fuel/maxFuel);
        }
    }

    private async void MoveToLowestLevel()
    {
        WaterTile currentTile = GetCurrentWaterTile();

        List<Tile> neighbours = currentTile.neighbourDictionary.Keys.ToList();

        WaterTile bestNeighbour= null;
        float bestNeighbourScore = int.MaxValue;

        // The lower the score, the better
        foreach (Tile neighbour in neighbours)
        {
            if (!(neighbour is WaterTile) 
                || (neighbour is DamTile && !(neighbour as DamTile).isOpen)) continue;

            Direction moveDirection = currentTile.neighbourDictionary[neighbour];
            float directionScore;

            if (moveDirection == lastDirection) directionScore = SameDirectionScoreModifier;
            else if (moveDirection.Opposite() == lastDirection) directionScore = OppositeDirectionScoreModifier;
            else directionScore = 0;

            float finalScore = (neighbour as WaterTile).waterLevel + directionScore;

            if (finalScore < bestNeighbourScore)
            {
                bestNeighbourScore = finalScore;
                bestNeighbour = neighbour as WaterTile;
            }
        }
/*
        WaterTile lowestNeighbour = currentTile.neighbourDictionary.Keys.Select(n => (WaterTile)n)
                                                                        .OrderBy(n => n.waterLevel)
                                                                        .First();*/

        if (bestNeighbourScore <= currentTile.waterLevel && bestNeighbour != null)
        {
            lastDirection = currentTile.neighbourDictionary[bestNeighbour];
            await MoveToPosition(bestNeighbour.Position.ToWorldPos());          // Boat should move to worldPosition!
            MoveToLowestLevel();
        }
    }

    private void OnEnable()
    {
        EventSystem.Subscribe(EventName.TILE_CLICKED, (value) => MovePath(value));
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe(EventName.TILE_CLICKED, (value) => MovePath(value));
    }

    public async void MovePath(object _value)
    {
        Vector3Int targetTilePos = (Vector3Int)_value;
        List<Vector3Int> path = _gridManager.CalculatePath(transform.position.ToVector3Int(), targetTilePos);

        foreach (Vector3Int tilePos in path)
        {
            await MoveToPosition(tilePos);
        }
    }

    public async Task MoveToPosition(Vector3 _tilePos)
    {
        _tilePos.y = transform.position.y;
        await MoveToInSeconds(transform.position, _tilePos, 1);

        CleanCurrentTile();
    }

    private WaterTile GetCurrentWaterTile()
    {
        return _gridManager.GetTile(GridCell.WorldPosToCellCord(transform.position.ToVector3Int())) as WaterTile;
    }

    private void CleanCurrentTile()
    {
        WaterTile currentTile = GetCurrentWaterTile();
        if (currentTile == null) return;

        if (!currentTile.isCleaned) AddFuel(10);
        currentTile.Clean();
    }
}
