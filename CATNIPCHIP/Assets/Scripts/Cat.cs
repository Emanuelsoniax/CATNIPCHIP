using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEditor.TerrainTools;
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

    private Waypoint pastIdlePoint;

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

    /// <summary>
    /// Sets the destination Waypoint as current and changes the past Idle point back.
    /// </summary>
    private void ArrivedAtWaypoint()
    {
        if(pastIdlePoint == Current)
        {
            Current.waypointType = Waypoint.WaypointType.IdlePoint;
        }
        waypointManager.SetNextWaypointAsCurrent();
        SetNextWaypoint();
    }

    /// <summary>
    /// Sets the next Waypoint in the collection and sets the destination in the Agent.
    /// </summary>
    private void SetNextWaypoint()
    {
        Debug.Log("Current Waypoint: " + Current.waypointType + "   Next Waypoint: " + waypointManager.NextWaypoint.waypointType);

        if (waypointManager.NextWaypoint.waypointType == Waypoint.WaypointType.IdlePoint) pastIdlePoint = waypointManager.NextWaypoint;

        agent.SetDestination(waypointManager);
    }

}
