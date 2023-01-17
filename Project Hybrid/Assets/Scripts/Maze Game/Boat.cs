using UnityEngine;
using System.Linq;
using MarcoHelpers;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Boat : MovingObject
{
    [Header("Movement & Animations")]
    [SerializeField] private float speed;
    [SerializeField] private float rotateTime;

    [SerializeField] private float speedPerLevelModifier = 1.2f;

    [Header("Behaviour")]
    [Tooltip("The lower the number, the more the ship will try to keep straight.")]
    [Range(-10, 0)]
    [SerializeField] private float sameDirectionScoreModifier = -1;
    [Tooltip("With higher numbers, the ship is less likely to turn around")]
    [Range(0, 10)]
    [SerializeField] private float oppositeDirectionScoreModifier = 1;
    [Tooltip("With higher numbers, the ship is more likely to go to a clean tile")]
    [Range(0, 10)]
    [SerializeField] private float cleanTilePenalty = 2;

    [Header("Fuel")]
    [SerializeField] private StatBar fuelBar;
    [SerializeField] private float maxFuel = 100;
    [SerializeField] private float tileFuelCost = 1;
    [SerializeField] private float tileCleanFuelBonus = 2;
    [SerializeField] private float waterClimbFuelCost = 5;

    private float currentSpeed;
    private bool isMoving;
    private float fuel;
    private GridManager _gridManager;
    private Direction lastDirection;

    private bool isPaused;

    private void Awake()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    private void Start()
    {
        currentSpeed = speed;
        fuel = maxFuel / 2;     // Start with half of the max fuel

        Vector3 newPos = transform.position;
        newPos.y = GetCurrentWaterTile().yPos;
        transform.position = newPos;

        CleanCurrentTile();
        MoveToLowestTile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveFuel(5);
        }
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

    public void RemoveFuel(float _amount)
    {
        fuel -= _amount;
        if (fuel <= 0)
        {
            isPaused = true;
            EventSystem.RaiseEvent(EventName.FUEL_EMPTY);
        }

        if (fuelBar != null)
        {
            fuelBar.SetSliderValue(Mathf.Clamp(fuel, 0, maxFuel) / maxFuel);
        }
    }

    private async void MoveToLowestTile()
    {
        if (isPaused) return;

        WaterTile currentTile = GetCurrentWaterTile();

        List<Tile> neighbours = currentTile.neighbourDictionary.Keys.ToList();

        float bestNeighbourScore;
        WaterTile bestNeighbour = CalculateBestNeighbour(currentTile, neighbours, out bestNeighbourScore);

        if (bestNeighbour != null)
        {
            isMoving = true;
            if (bestNeighbourScore > currentTile.waterLevel)
            {
                RemoveFuel(waterClimbFuelCost);
            }
            else
            {
                RemoveFuel(tileFuelCost);
            }

            Direction moveDirection = currentTile.neighbourDictionary[bestNeighbour];
            await RotateBoat(moveDirection);

            int waterDifference = bestNeighbour.waterLevel - currentTile.waterLevel;
            CalculateCurrentSpeed(waterDifference);

            lastDirection = moveDirection;
            await MoveToPosition(bestNeighbour.Position.ToWorldPos());          // Boat should move to worldPosition!
            MoveToLowestTile();
        }
        else
        {
            isMoving = false;
        }
    }

    private void CalculateCurrentSpeed(int waterDifference)
    {
        if (waterDifference > 0)
        {
            currentSpeed = speed / Mathf.Pow(speedPerLevelModifier, Mathf.Abs(waterDifference));
        }
        else
        {
            currentSpeed = speed * Mathf.Pow(speedPerLevelModifier, Mathf.Abs(waterDifference));
        }
    }

    private async Task RotateBoat(Direction _moveDirection)
    {
        if (_moveDirection != lastDirection)
        {
            float rotateTimeModifier = _moveDirection == lastDirection.Opposite() ? 2f : 1f;
            FindObjectOfType<FollowTarget>().RotateWithPlayer(_moveDirection, rotateTimeModifier);
            await RotateTowardsInSeconds(transform.rotation, _moveDirection.GetRotation(), rotateTime * rotateTimeModifier);
        }
    }

    private WaterTile CalculateBestNeighbour(WaterTile currentTile, List<Tile> neighbours, out float bestNeighbourScore)
    {
        WaterTile bestNeighbour = null;
        bestNeighbourScore = int.MaxValue;

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

        return bestNeighbour;
    }

/*    private void OnEnable()
    {
        EventSystem.Subscribe(EventName.TILE_CLICKED, (value) => MovePath(value));
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe(EventName.TILE_CLICKED, (value) => MovePath(value));
    }*/

/*    public async void MovePath(object _value)
    {
        Vector3Int targetTilePos = (Vector3Int)_value;
        List<Vector3Int> path = _gridManager.CalculatePath(transform.position.ToVector3Int(), targetTilePos);

        foreach (Vector3Int tilePos in path)
        {
            await MoveToPosition(tilePos);
        }
    }*/

    public async Task MoveToPosition(Vector3 _tilePos)
    {
        _tilePos.y = GetCurrentWaterTile().yPos;
        await MoveToInSeconds(transform.position, _tilePos, 1/currentSpeed);

        //AdjustHeight();
        CleanCurrentTile();
    }

    private void AdjustHeight()
    {
        Vector3 newPos = transform.position;
        newPos.y = GetCurrentWaterTile().yPos;
        //MoveToInSeconds(transform.position, newPos, 0.2f);

        //transform.position = newPos;
    }

    private WaterTile GetCurrentWaterTile()
    {
        return _gridManager.GetTile(GridCell.WorldPosToCellCord(transform.position.ToVector3Int())) as WaterTile;
    }

    private void CleanCurrentTile()
    {
        WaterTile currentTile = GetCurrentWaterTile();
        if (currentTile == null) return;

        if (!currentTile.isCleaned) AddFuel(tileCleanFuelBonus);
        currentTile.Clean();
    }
}
