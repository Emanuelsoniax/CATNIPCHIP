#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(WorldTransitionManager))]
public class WorldTransitionManagerEditor : Editor
{
    private WorldTransitionManager _target;
    private WorldState _debugState;

    private void OnEnable()
    {
        _target = (WorldTransitionManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying)
            return;

        _debugState = (WorldState)EditorGUILayout.EnumPopup("State", _debugState);

        if (GUILayout.Button("Set World State"))
            _target.CurrentState = _debugState;
    }
}

#endif