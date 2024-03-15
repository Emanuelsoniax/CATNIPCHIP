using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.EditorTools;


[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var waypoint = target as Waypoint;

        switch (waypoint.waypointType)
        {
            case Waypoint.WaypointType.IdlePoint:
                waypoint.state = (IdleState)EditorGUILayout.ObjectField("Behaviour State",waypoint.state, typeof(IdleState), true);
                waypoint.nextState = (MovingState)EditorGUILayout.ObjectField("Behaviour State After Event",waypoint.nextState, typeof(MovingState), true);
                break;
            case Waypoint.WaypointType.PassThroughPoint:
                waypoint.state = (MovingState)EditorGUILayout.ObjectField("Behaviour State",waypoint.state, typeof(MovingState), true);
                break;
        }
    }
}
