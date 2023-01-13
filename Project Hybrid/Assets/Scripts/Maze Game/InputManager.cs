using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    /*private bool dam1Open;
    private bool dam2Open;
    private bool dam3Open;
    private bool dam4Open;
    private bool dam5Open;*/

    private Dictionary<int, bool> damOpenDictionary = new Dictionary<int, bool>();
    private Dictionary<int, DamTile> damTiles = new Dictionary<int, DamTile>();

    private void Awake()
    {
        ServiceLocator.RegisterService<InputManager>(this);

        DamTile[] dams = FindObjectsOfType<DamTile>();
        foreach(DamTile dam in dams)
        {
            damTiles.Add(dam.id, dam);
        }
    }

    private void Start()
    {
        damOpenDictionary.Add(1, false);
        damOpenDictionary.Add(2, false);
        damOpenDictionary.Add(3, false);
        damOpenDictionary.Add(4, false);
        damOpenDictionary.Add(5, false);
    }

    public void ReceiveDamValue(int _id, bool _value)
    {
        if (damOpenDictionary.ContainsKey(_id) && damOpenDictionary[_id] != _value)
        {
            damOpenDictionary[_id] = _value;
            OnDamChanged(_id, _value);
        }
    }

    private void OnDamChanged(int _id, bool _value)
    {
        Debug.Log($"Dam {_id} Changed to {_value}");

        if (damTiles.ContainsKey(_id))
        {
            DamTile tile = damTiles[_id];
            tile.Trigger();
        }

    }
}
