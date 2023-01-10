using UnityEngine;
using MarcoHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Boat : MovingObject
{
    private GridManager _gridManager;

    private void Awake()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    private void Start()
    {
        CleanCurrentTile();
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
        await MoveToInSeconds(transform.position, _tilePos, .3f);

        CleanCurrentTile();
    }

    private void CleanCurrentTile()
    {
        WaterTile currentTile = _gridManager.GetTile(transform.position.ToVector3Int()) as WaterTile;
        currentTile?.Clean();
    }


}
