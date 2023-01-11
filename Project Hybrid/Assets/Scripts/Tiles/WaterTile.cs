using UnityEditor;
using UnityEngine;

public class WaterTile : Tile
{
    public Material waterMaterial;  // Temp
    public int waterLevel;

    private new MeshRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        //waterLevel = (int) Random.Range(1, 6);
    }

    public void Clean()
    {
        if (renderer == null) return;
        renderer.material = waterMaterial;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(position, waterLevel.ToString());
    }
}
