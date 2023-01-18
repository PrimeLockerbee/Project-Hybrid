using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WaterTile : Tile
{
    public Material minimapWaterMat;  // Temp
    public Material realWaterMaterial; 
    public int waterLevel { get { return level; } set { level = value; OnLevelChanged(level); } }

    public int maxLevel = 100;
    public bool isCleaned;
    public bool isSpawnPos;

    [HideInInspector] public float yPos;
    [HideInInspector] private TrashSpawner[] trash;

    [SerializeField] private float animationTime;

    [SerializeField] private MeshRenderer minimapRenderer;
    [SerializeField] private MeshRenderer waterRenderer;
    private int level;

    protected virtual void Awake()
    {
        trash = GetComponentsInChildren<TrashSpawner>();
        if (minimapRenderer != null || waterRenderer != null) return;

        minimapRenderer = GetComponent<MeshRenderer>();
        waterRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Start()
    {
        if (isCleaned && minimapRenderer != null)
        {
            minimapRenderer.material = minimapWaterMat;
        }

        Invoke(nameof(CheckIfIsClean), 0.1f);
    }

    private void CheckIfIsClean()
    {
        if (trash.Where(t => t.didSpawn).Count() == 0)
        {
            Clean();
        }
    }

    private async void OnLevelChanged(int _waterLevel)
    {
        //Debug.Log("Changed!");
        Vector3 newPos = transform.position;
        yPos = level * .5f - 0.5f;
        newPos.y = yPos;
        //await MoveToInSeconds(transform.position, newPos, animationTime * 2);

        transform.position = newPos;
    }

    public virtual void Clean()
    {
        isCleaned = true;

        foreach (TrashSpawner spawner in trash)
        {
            spawner.gameObject.SetActive(false);
        }

        if (minimapRenderer != null)
        {
            minimapRenderer.material = minimapWaterMat;
        }

        if (waterRenderer != null)
        {
            waterRenderer.material = realWaterMaterial;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position.ToVector3Int(), waterLevel.ToString());
    }
}
