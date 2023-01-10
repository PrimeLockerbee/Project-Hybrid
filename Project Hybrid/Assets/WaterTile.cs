using Oculus.Interaction.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaterTile : Tile
{
    public Material waterMaterial;  // Temp
    public int waterLevel;

    private MeshRenderer renderer;

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
        renderer.material = waterMaterial;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(position, waterLevel.ToString());
    }
}
