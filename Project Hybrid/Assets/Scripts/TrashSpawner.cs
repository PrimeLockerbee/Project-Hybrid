using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MovingObject
{
    [Range(0, 1)] public float SpawnChance = 0.5f;
    public List<GameObject> possibleTrashPrefabs;

    public bool didSpawn;

    private void Start()
    {
        float random = Random.Range(0f, 1f);

        if (random <= SpawnChance)
        {
            didSpawn = true;
            GameObject randomTrashAsset = possibleTrashPrefabs[Random.Range(0, possibleTrashPrefabs.Count)];
            Quaternion randomRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Instantiate(randomTrashAsset, transform.position, randomRotation, transform);
        }
    }
}
