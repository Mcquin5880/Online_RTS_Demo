using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMover unitMover = null;
    [SerializeField] private UnityEvent onUnitSelected = null;
    [SerializeField] private UnityEvent onUnitDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public UnitMover GetUnitMover()
    {
        return this.unitMover;
    }

    #region Server

    public override void OnStartServer()
    {
        ServerOnUnitSpawned.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned.Invoke(this);
    }

    #endregion


    #region Client

    public override void OnStartClient()
    {
        if (isClientOnly && hasAuthority)
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }
    }

    public override void OnStopClient()
    {
        if (isClientOnly && hasAuthority)
        {
            AuthorityOnUnitDespawned?.Invoke(this);
        }
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }

        onUnitSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }

        onUnitDeselected?.Invoke();
    }

    #endregion

}
