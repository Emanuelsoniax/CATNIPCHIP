using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Cat : MonoBehaviour
{
    [SerializeField]
    private WaypointManager waypointManager = new WaypointManager();
    [SerializeField]
    private SmoothAgentMovement agent;

    [SerializeField]
    private Waypoint current;

    private void Start()
    {
        transform.position = waypointManager.CurrentWaypoint.Position;
        agent.OnDestinationReached += ArrivedAtWaypoint;
        SetNextWaypoint();
    }

    private void Update()
    {
        current = waypointManager.CurrentWaypoint;

        Debug.Log(waypointManager.CanMoveToNextWaypoint);

        if (waypointManager.CanMoveToNextWaypoint)
        {
           agent.MoveAgent();
        }

        if(Vector3.Distance(transform.position, waypointManager.CurrentWaypoint.Position) <= 0.01f)
        {
            Debug.Log("aaaa");
            SetNextWaypoint();
        }
    }

    private void ArrivedAtWaypoint()
    {
        waypointManager.SetNextWaypointAsCurrent();
        SetNextWaypoint();
        Debug.Log("juist ja");
    }

    private void SetNextWaypoint()
    {
        agent.SetDestination(waypointManager);
    }

}
