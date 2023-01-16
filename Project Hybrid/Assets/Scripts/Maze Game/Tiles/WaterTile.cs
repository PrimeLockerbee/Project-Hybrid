using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class WaterTile : Tile
{
    public Material waterMaterial;  // Temp
    public int waterLevel { get { return level; } set { level = value; OnLevelChanged(level); } }

    public int maxLevel = 100;
    public bool isCleaned;

    public float yPos;

    [SerializeField] private float animationTime;

    private new MeshRenderer renderer;
    private int level;

    protected virtual void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        if (isCleaned && renderer != null)
        {
            renderer.material = waterMaterial;
        }
    }

    private void OnLevelChanged(int _waterLevel)
    {
        Vector3 newPos = transform.position;
        yPos = _waterLevel * 2f - 0.5f;
        newPos.y = yPos;
        //MoveToInSeconds(transform.position, newPos, animationTime);

        transform.position = newPos;
    }

    public virtual void Clean()
    {
        isCleaned = true;

        if (renderer == null) return;
        renderer.material = waterMaterial;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position.ToVector3Int(), waterLevel.ToString());
    }
}
