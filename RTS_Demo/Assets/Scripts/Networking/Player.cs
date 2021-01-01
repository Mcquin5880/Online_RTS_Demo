using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private List<Unit> playerUnits = new List<Unit>();

    public List<Unit> GetPlayerUnits()
    {
        return this.playerUnits;
    }

    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (ConnectionIdMatches(unit))
        {
            playerUnits.Add(unit);
        }
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (ConnectionIdMatches(unit))
        {
            playerUnits.Remove(unit);
        }
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region Client

    public override void OnStartClient()
    {
        if (isClientOnly)
        {
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }
    }

    public override void OnStopClient()
    {
        if (isClientOnly)
        {
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        if (hasAuthority) 
        {
            playerUnits.Add(unit);
        }
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (hasAuthority)
        {
            playerUnits.Remove(unit);
        }
    }

    #endregion

    // Check that a specific unit's connection id matches the connection id for this player

    private bool ConnectionIdMatches(Unit unit)
    {
        return unit.connectionToClient.connectionId == connectionToClient.connectionId;
    }
}
