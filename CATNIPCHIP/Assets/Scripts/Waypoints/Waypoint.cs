using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;


public class Waypoint : MonoBehaviour
{
    public enum WaypointType { IdlePoint, PassThroughPoint, JumpPoint }
    [SerializeField]
    public WaypointType waypointType;
    [HideInInspector]
    public BaseState state;
    [HideInInspector]
    public BaseState nextState;

    public bool eventHappened = false;
    public event Action<BaseState> onWaitedForEvent;

    private void Start()
    {
        //TODO: Listen to events that are a condition
    }


    public Vector3 Position
    {
        get { return transform.position; } 
    } 

    public IEnumerator WaitForEvent(Func<bool> condition)
    {
        yield return new WaitUntil(() => condition());

        onWaitedForEvent?.Invoke(nextState);
        state = nextState;
        waypointType = WaypointType.PassThroughPoint;
        eventHappened = false;

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
        Gizmos.DrawSphere(transform.position, 0.3f);
    }

    public void EventHappened()
    {
        eventHappened = true;
    }

}
