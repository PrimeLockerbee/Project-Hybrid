using UnityEditor;
using UnityEngine;

public class WaterTile : Tile
{
    public Material waterMaterial;  // Temp
    public int waterLevel;
    public int maxLevel = 100;
    public bool isCleaned;

    private new MeshRenderer renderer;

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
