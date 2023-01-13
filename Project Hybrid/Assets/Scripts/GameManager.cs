using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject prefab;

    private void Awake()
    {
        Instantiate(prefab);
    }

}
