using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class WaypointManager
{
    [System.Serializable]
    public class WaypointCollection
    {
        public Waypoint[] waypoints;
    }

    [SerializeField]
    public List<WaypointCollection> waypointCollections;
    private int currentCollection = 0;
    public WaypointCollection CurrentCollection
    {
        get { return waypointCollections[currentCollection]; }
    }
    private int currentWaypoint = 0;
    public Waypoint CurrentWaypoint
    {
        get { return waypointCollections[currentCollection].waypoints[currentWaypoint]; }
    }

    public Waypoint NextWaypoint
    {
        get { return GetNextWaypoint(); }
    }

    public bool CanMoveToNextWaypoint
    {
        get
        {
            switch (CurrentWaypoint.waypointType)
            {
                case Waypoint.WaypointType.PassThroughPoint:
                    return true;
                case Waypoint.WaypointType.IdlePoint:
                    return false;
                case Waypoint.WaypointType.JumpPoint:
                    return false;
                default:
                    Debug.Log("Can't determine if the agent can move");
                    return false;
            }
        }
    }

    public void SetNextWaypointAsCurrent()
    {

        if(currentWaypoint < waypointCollections[currentCollection].waypoints.Length)
        {
            currentWaypoint++;
        }else if(currentWaypoint >= waypointCollections[currentCollection].waypoints.Length)
        {
            currentCollection ++;
            currentWaypoint = 0;
        }

        if (CurrentWaypoint.waypointType == Waypoint.WaypointType.IdlePoint)
        {
            CurrentWaypoint.StartCoroutine(CurrentWaypoint.WaitForEvent(KeyCode.K));
        }

    }

    public Waypoint GetNextWaypoint()
    {
        Waypoint waypoint = null;

        if (currentWaypoint < waypointCollections[currentCollection].waypoints.Length -1)
        {
            waypoint = waypointCollections[currentCollection].waypoints[currentWaypoint + 1];
        }
        else if (currentWaypoint >= waypointCollections[currentCollection].waypoints.Length)
        {
            waypoint = waypointCollections[currentCollection + 1].waypoints[0];
        }

        return waypoint;
    }

}
