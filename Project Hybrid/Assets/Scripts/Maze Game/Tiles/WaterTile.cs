using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

    [SerializeField] protected GameObject stromingFX;
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
        yPos = transform.position.y;
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
        if (stromingFX == null) return;

        WaterTile highestNeighbour = null;
        WaterTile tiedNeighbour = null;
        List<WaterTile> neighbours = neighbourList.Select(t => t as WaterTile)
                                                    .Where(t => t != null)
                                                    .OrderByDescending(t => t.waterLevel)
                                                    .ToList();

        if (neighbours.Count() > 0 )
        {
            highestNeighbour= neighbours[0];

            if (neighbours.Count() > 1 && highestNeighbour.waterLevel == neighbours[1].waterLevel)
            {
                tiedNeighbour = neighbours[1];
            }
        }

        if (highestNeighbour != null && tiedNeighbour == null)
        {
            Direction neighbourDirection = neighbourDictionary[highestNeighbour];
            stromingFX.transform.rotation = neighbourDirection.GetRotation() * Quaternion.AngleAxis(90, Vector3.up);
        }
        else if (highestNeighbour != null && tiedNeighbour != null)
        {
            Direction neighbourDirection1 = neighbourDictionary[highestNeighbour];
            Direction neighbourDirection2 = neighbourDictionary[tiedNeighbour];
            if (neighbourDirection1 != neighbourDirection2.Opposite())
            {
                stromingFX.transform.rotation = Quaternion.Slerp(neighbourDirection1.GetRotation(), neighbourDirection2.GetRotation(), 0.5f); //* Quaternion.AngleAxis(90, Vector3.up);
            }
        }
        /*        Vector3 newPos = transform.position;
                yPos = level * .5f - 0.5f;
                newPos.y = yPos;
                //await MoveToInSeconds(transform.position, newPos, animationTime * 2);

                transform.position = newPos;*/
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

/*    private void OnDrawGizmos()
    {
        Handles.Label(transform.position.ToVector3Int(), waterLevel.ToString());
    }*/
}
