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

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;


    private void Awake()
    {
        animator.applyRootMotion = true;
        agent.agent.updatePosition = true;
        agent.agent.updateRotation = false;
    }

    private void Start()
    {
        transform.position = Current.Position;
        OnStart();
        agent.OnDestinationReached += ArrivedAtWaypoint;
        SetNextWaypoint();
    }

    private void Update()
    {
        UpdateState(Current.state);

        UpdateAnimation();

        if (waypointManager.CanMoveToNextWaypoint)
        {
           agent.MoveAgent();
        
        }

    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.agent.nextPosition.y;
        transform.position = rootPosition;
        transform.rotation = animator.rootRotation;
        agent.agent.nextPosition = rootPosition;
    }

    private void UpdateAnimation()
    {
       Vector3 worldDeltaPosition = agent.agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime/0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = smoothDeltaPosition / Time.deltaTime;

        if(agent.agent.remainingDistance <= agent.agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero,velocity,agent.agent.remainingDistance/ agent.agent.stoppingDistance);
        }

        bool move = agent.agent.velocity.magnitude > 0.5f && agent.agent.remainingDistance > agent.agent.stoppingDistance;

        animator.SetBool("Move", waypointManager.CanMoveToNextWaypoint);
        animator.SetFloat("VelocityX", agent.agent.velocity.normalized.x);
        animator.SetFloat("VelocityZ", agent.agent.velocity.normalized.z);

        Debug.Log(velocity.x);

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if(deltaMagnitude > agent.agent.radius /2f)
        {
            transform.position = Vector3.Lerp(animator.rootPosition, agent.agent.nextPosition, smooth);
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
