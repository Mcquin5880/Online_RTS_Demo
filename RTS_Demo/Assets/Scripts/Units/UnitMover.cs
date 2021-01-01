using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMover : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent = null;

    #region Server

    [ServerCallback]
    private void Update()
    {
        if (!navMeshAgent.hasPath) return;
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return;
        navMeshAgent.ResetPath();
        
    }

    [Command]
    public void MovementCommand(Vector3 pos)
    {
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    #endregion
}
