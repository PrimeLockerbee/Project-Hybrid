using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialColor : MonoBehaviour
{
    private void Awake()
    {
        MeshRenderer bottleRenderer = GetComponent<MeshRenderer>();
        Material material = bottleRenderer.materials[0];
        material.color = new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1), 1);
        bottleRenderer.material = material;
    }
}
