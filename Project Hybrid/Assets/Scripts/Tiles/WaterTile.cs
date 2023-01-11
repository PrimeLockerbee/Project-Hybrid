using UnityEditor;
using UnityEngine;

public class WaterTile : Tile
{
    public Material waterMaterial;  // Temp
    public int waterLevel;
    public bool isCleaned;

    private new MeshRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        if (isCleaned && renderer != null)
        {
            renderer.material = waterMaterial;
        }
        //waterLevel = (int) Random.Range(1, 6);
    }

    public void Clean()
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
