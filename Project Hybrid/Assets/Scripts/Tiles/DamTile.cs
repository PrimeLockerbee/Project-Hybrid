using System;
using UnityEngine;

public class DamTile : WaterTile
{
    public bool isOpen;
    public event Action<DamTile> OnOpen;
    public event Action<DamTile> OnClose;

    private Dam damObject;

    private void Awake()
    {
        damObject = GetComponentInChildren<Dam>();
    }

    public void Open()
    {
        isOpen = true;
        damObject.MoveUp();
        OnOpen?.Invoke(this);
    }

    public void Close()
    {
        isOpen = false;
        damObject.MoveDown();
        OnClose?.Invoke(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isOpen) Close();
            else Open();
        }
    }
}
