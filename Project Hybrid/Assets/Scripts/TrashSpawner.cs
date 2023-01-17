using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MovingObject
{
    [Range(0, 1)] public float SpawnChance = 0.5f;
    public GameObject prefab;

    private void Start()
    {
        Quaternion randomRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        Instantiate(prefab, transform.position, randomRotation, transform);
    }
}
