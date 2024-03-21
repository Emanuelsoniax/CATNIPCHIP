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
    [SerializeField]
    private SmoothAgentMovement agent;

    private Waypoint Current
    {
        get { return waypointManager.CurrentWaypoint; }
    }

    private Waypoint pastIdlePoint;


    private void Awake()
    {
        animator.applyRootMotion = true;
        agent.agent.updateRotation = false;
        agent.agent.updatePosition = true;
        transform.position = Current.Position;
        currentBehaviorState = Current.state;
    }

    private void OnEnable()
    {
        agent.OnDestinationReached += UpdateState;
        agent.OnDestinationReached += ArrivedAtWaypoint;
    }

    private void Start()
    {
        agent.agent.updatePosition = false;
        OnStart();
        SetNextWaypoint();
    }

    private void Update()
    {
        OnUpdate();

        if (waypointManager.CanMoveToNextWaypoint)
        {
           agent.MoveAgent();
        
        }

    }

    public override void UpdateState()
    {
        SwitchState(waypointManager.NextWaypoint.state);
        base.UpdateState();
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

        if (waypointManager.NextWaypoint.waypointType == Waypoint.WaypointType.IdlePoint)
        {
            pastIdlePoint = waypointManager.NextWaypoint;
            pastIdlePoint.onWaitedForEvent += UpdateState;
        }

        agent.SetDestination(waypointManager);
    }

}
