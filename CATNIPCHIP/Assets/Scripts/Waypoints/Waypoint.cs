using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Waypoint : MonoBehaviour
{
    private enum WaypointType { IdlePoint, PassThroughPoint }
    [SerializeField]
    private WaypointType waypointType;

    private void OnDrawGizmos()
    {
        Color gizmosColor;

        switch (waypointType)
        {
            case WaypointType.IdlePoint:
                gizmosColor = Color.red;
                break;
            case WaypointType.PassThroughPoint:
                gizmosColor = Color.blue;
                break;
            default: 
                gizmosColor = Color.white;
                break;
        }

        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(transform.position, 0.25f);

    }
}
