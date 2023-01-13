using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DamTile : WaterTile
{
    public event Action<DamTile> OnOpen;
    public event Action<DamTile> OnClose;

    [Header("Dam")]
    public bool isOpen;
    public int id = -1;

    // Private Fields
    private Task currentTask;
    private Action awaitedCall;
    private Dam damObject;
    private bool isMoving;

    protected override void Awake()
    {
        base.Awake();
        damObject = GetComponentInChildren<Dam>();
    }

    public void Trigger()
    {
        if (isMoving)
        {
            if (awaitedCall == null)
            {
                if (isOpen) { awaitedCall = Close; }
                else awaitedCall = Open;
            }
            else if (isOpen && awaitedCall == Open)
            {
                // Change awaiting call to close
                awaitedCall = Close;
            }
            else if (!isOpen && awaitedCall == Close)
            {
                // Change awaiting call to open
                awaitedCall = Open;
            }
        }
        else
        {
            if (isOpen) Close();
            else Open();
        }
    }

    public async void Open()
    {
        isOpen = true;
        isMoving = true;
        currentTask = damObject.MoveUp();
        OnOpen?.Invoke(this);
    }

    public async void Close()
    {
        isOpen = false;
        isMoving = true;
        currentTask = damObject.MoveDown();
        OnClose?.Invoke(this);
    }

    public override void Clean()
    {
        base.Clean();
    }

    private void Update()
    {
        if (currentTask != null && currentTask.IsCompleted)
        {
            isMoving = false;
            currentTask = null;
            awaitedCall?.Invoke();
            awaitedCall = null;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Trigger();
        }
    }
}
