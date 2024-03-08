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

    public void SetNextWaypoint()
    {
        if(currentWaypoint < waypointCollections[currentCollection].waypoints.Length)
        {
            currentWaypoint++;
        }else if(currentWaypoint >= waypointCollections[currentCollection].waypoints.Length)
        {
            currentCollection ++;
            currentWaypoint = 0;
        } 
    }

    public Waypoint GetNextWaypoint()
    {
        Waypoint waypoint = null;

        if (currentWaypoint < waypointCollections[currentCollection].waypoints.Length)
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
