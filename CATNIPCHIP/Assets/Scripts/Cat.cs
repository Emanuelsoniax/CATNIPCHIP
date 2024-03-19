using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Cat : StateMachine
{
    [SerializeField]
    private WaypointManager waypointManager = new WaypointManager();
    private SmoothAgentMovement agent;

    private Waypoint Current
    {
        get { return waypointManager.CurrentWaypoint; }
    }

    private void Start()
    {
        agent = GetComponent<SmoothAgentMovement>();
        transform.position = Current.Position;
        OnStart();
        agent.OnDestinationReached += ArrivedAtWaypoint;
        SetNextWaypoint();
    }

    private void Update()
    {
        UpdateState(Current.state);

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
        Debug.Log("Current Waypoint: " + Current.waypointType + "   Next Waypoint: " + waypointManager.NextWaypoint.waypointType);
        agent.SetDestination(waypointManager);
    }

}
