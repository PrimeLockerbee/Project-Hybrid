using Oculus.Interaction.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : Tile
{
    public Material water;
    private MeshRenderer renderer;

    private void Awake()
    {
        renderer= GetComponent<MeshRenderer>();
    }

    public void Clean()
    {
        renderer.material= water;
    }
}
