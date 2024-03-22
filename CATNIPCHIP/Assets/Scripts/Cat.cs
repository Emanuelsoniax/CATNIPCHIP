using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;


[RequireComponent (typeof(CatController))]
public class Cat : MonoBehaviour
{
    [SerializeField]
    private WaypointManager waypointManager = new WaypointManager();
    [SerializeField]
    private SmoothAgentMovement agent;
    [SerializeField]
    public CatController controller;
    [HideInInspector]
    public StateMachine stateMachine = new StateMachine();

    private Waypoint Current
    {
        get { return waypointManager.CurrentWaypoint; }
    }

    private Waypoint pastIdlePoint;

    private void OnValidate()
    {
        if(!controller) { controller = GetComponent<CatController>(); }
    }

    private void Awake()
    {
        transform.position = Current.Position;
        stateMachine.currentBehaviorState = Current.state;
    }

    private void OnEnable()
    {
        controller.OnDestinationReached += UpdateState;
        controller.OnDestinationReached += ArrivedAtWaypoint;
    }

    private void Start()
    {
        stateMachine.OnStart();
        SetNextWaypoint();
    }

    private void Update()
    {
        stateMachine.OnUpdate();

        if (waypointManager.CanMoveToNextWaypoint)
        {
            controller.Move();
        }

    }

    private void UpdateState()
    {
        stateMachine.SwitchState(waypointManager.NextWaypoint.state);
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
            pastIdlePoint.onWaitedForEvent += stateMachine.UpdateState;
        }

        controller.SetDestination(waypointManager.NextWaypoint);
        //agent.SetDestination(waypointManager);
    }

}
