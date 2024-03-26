using System;
using System.Collections;
using UnityEngine;

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

    private void OnEnable()
    {
        (state as IdleState).onWaitedForEvent += EventHappened;
        
    }

    private void OnDisable()
    {
        (state as IdleState).onWaitedForEvent -= EventHappened;
    }


    public Vector3 Position
    {
        get { return transform.position; }
    }

    public IEnumerator WaitForEvent(Func<bool> condition)
    {
        Coroutine routine = StartCoroutine((state as IdleState).InteractionTimer());

        yield return new WaitUntil(() => condition());

        StopCoroutine(routine);
        state = nextState;
        onWaitedForEvent?.Invoke(nextState);
        waypointType = WaypointType.PassThroughPoint;
        eventHappened = false;

        yield return null;
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
