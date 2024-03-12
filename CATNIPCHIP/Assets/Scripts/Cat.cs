using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Cat : StateMachine
{
    [SerializeField]
    private WaypointManager waypointManager = new WaypointManager();
    [SerializeField]
    private SmoothAgentMovement agent;

    [SerializeField]
    private Waypoint Current
    {
        get { return waypointManager.CurrentWaypoint; }
    }

    private void Start()
    {
        transform.position = waypointManager.CurrentWaypoint.Position;
        agent.OnDestinationReached += ArrivedAtWaypoint;
        SetNextWaypoint();
    }

    private void Update()
    {
        if (waypointManager.CanMoveToNextWaypoint)
        {
           agent.MoveAgent();
        }
    }

    private void ArrivedAtWaypoint()
    {
        waypointManager.SetNextWaypointAsCurrent();
        SetNextWaypoint();
    }

    private void SetNextWaypoint()
    {
        agent.SetDestination(waypointManager);
    }

    public override void IdleBehaviour()
    {
        base.IdleBehaviour();
    }

}
