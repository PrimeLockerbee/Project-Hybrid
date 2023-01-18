using MarcoHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DamTile : WaterTile
{
    public event Action<DamTile> OnOpen;
    public event Action<DamTile> OnClose;

    [Header("Dam")]
    [SerializeField] public Dam damObject;
    public bool isOpen;
    public int id = -1;
    [SerializeField] private MeshRenderer minimapDamRend;
    [SerializeField] private Material minimapDamClosed;
    [SerializeField] private Material minimapDamOpen;

    // Private Fields
    private Task currentTask;
    private Action awaitedCall;
    private bool isMoving;

    private bool isChanged;

    protected override void Awake()
    {
        base.Awake();
        if (damObject == null)
        {
            damObject = GetComponentInChildren<Dam>();
        }
    }

    private void OnEnable()
    {
        EventSystem.Subscribe(EventName.DAM_CHANGED, (value) => OnDamChanged(value));
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe(EventName.DAM_CHANGED, (value) => OnDamChanged(value));
    }

    public void OnDamChanged(object _value)
    {
        Debug.Log("Stap 2");
        int changedDamID = (int)_value;
        if (changedDamID == id) isChanged = true;
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
        //isMoving = true;

        minimapDamRend.material = minimapDamOpen;
        /*currentTask = */
        //damObject.gameObject.SetActive(false);
        await damObject.MoveUp();
        isOpen = true;
        OnOpen?.Invoke(this);
    }

    public async void Close()
    {
        //isMoving = true;
        minimapDamRend.material = minimapDamClosed;

        //damObject.gameObject.SetActive(true);

        /*currentTask = */
        await damObject.MoveDown();
        isOpen = false;
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

        if (Input.GetKeyDown(KeyCode.Return) || isChanged)
        {
            Trigger();
            isChanged = false;
        }
    }
}
