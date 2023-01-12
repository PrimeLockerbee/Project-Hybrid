using UnityEngine;
using System.Linq;
using MarcoHelpers;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Boat : MovingObject
{
    [SerializeField] private StatBar fuelBar;
    [SerializeField] private float speed;
    [SerializeField] private float rotateTime;

    [Tooltip("The lower the number, the more the ship will try to keep straight.")]
    [Range(-10, 0)]
    [SerializeField] private float sameDirectionScoreModifier = -1;
    [Tooltip("With higher numbers, the ship is less likely to turn around")]
    [Range(0, 10)]
    [SerializeField] private float oppositeDirectionScoreModifier = 1;
    [Tooltip("With higher numbers, the ship is less likely to go to a clean tile")]
    [Range(0, 10)]
    [SerializeField] private float cleanTilePenalty = 2;

    [SerializeField] private float maxFuel = 100;

    private bool isMoving;
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

            // Direction Penalty
            Direction moveDirection = currentTile.neighbourDictionary[neighbour];
            float directionScore;

            if (moveDirection == lastDirection) directionScore = sameDirectionScoreModifier;
            else if (moveDirection.Opposite() == lastDirection) directionScore = oppositeDirectionScoreModifier;
            else directionScore = 0;

            // Clean tile
            float cleanPenalty = (neighbour as WaterTile).isCleaned ? cleanTilePenalty : 0;

            float finalScore = (neighbour as WaterTile).waterLevel + directionScore + cleanPenalty;

            if (finalScore < bestNeighbourScore)
            {
                bestNeighbourScore = finalScore;
                bestNeighbour = neighbour as WaterTile;
            }
        }

        if (bestNeighbour != null)
        {
            isMoving = true;
            if (bestNeighbourScore <= currentTile.waterLevel)
            {
                AddFuel(-5);
            }
            Direction moveDirection = currentTile.neighbourDictionary[bestNeighbour];

            if (moveDirection != lastDirection)
            {
                RotateTowardsInSeconds(transform.rotation, moveDirection.GetRotation(), rotateTime);
                FindObjectOfType<FollowTarget>().RotateTowardsInSeconds(transform.rotation, moveDirection.GetRotation(), 2*rotateTime);
            }

            lastDirection = moveDirection;
            await MoveToPosition(bestNeighbour.Position.ToWorldPos());          // Boat should move to worldPosition!
            MoveToLowestLevel();
        }
        else
        {
            isMoving = false;
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
        await MoveToInSeconds(transform.position, _tilePos, 1/speed);

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
