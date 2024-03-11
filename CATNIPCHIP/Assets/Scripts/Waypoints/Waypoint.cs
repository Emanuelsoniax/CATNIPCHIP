using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;


public class Waypoint : MonoBehaviour
{
    public enum WaypointType { IdlePoint, PassThroughPoint, JumpPoint }
    [SerializeField]
    public WaypointType waypointType;
    public Vector3 Position
    {
        get { return transform.position; } 
    } 

    public IEnumerator WaitForEvent(KeyCode key)
    {
        yield return new WaitUntil(() => Input.GetKeyDown(key));

        waypointType = WaypointType.PassThroughPoint;

        yield return false;
    }

    public IEnumerator WaitForEvent(Vector3 position)
    {
        yield return new WaitUntil(() => transform.position == position);

        waypointType = WaypointType.PassThroughPoint;

        yield return false;
    }

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
            case WaypointType.JumpPoint:
                 gizmosColor = Color.green;
                 break;
            default: 
                gizmosColor = Color.white;
                break;
        }

        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(transform.position, 0.25f);

    }
}
