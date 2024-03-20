using CATNIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldState
{
    Dystopia,
    Utopia
}

[ExecuteAlways]
public class WorldTransitionManager : MonoBehaviour
{
    [SerializeField] private RadialMaskManager _radialMaskManager;
    [SerializeField] private float _worldTransitionTime = 5.0f;
    [SerializeField] private float _worldTransitionDist = 1000.0f;

    public RadialMaskManager RadialMaskManager => _radialMaskManager;

    public WorldState CurrentState
    {
        get => _currentState;
        set => TransitionWorldState(value);
    }

    public event Action<WorldState> onWorldTransitioned;

    private WorldState _currentState;

    private void TransitionWorldState(WorldState state)
    {
        switch (state)
        {
            case WorldState.Dystopia:
                if (!RadialMaskManager.TryGetPrimaryMask(out RadialMask mask))
                    return;

                RadialMaskManager.AnimatePrimaryMask(mask.BaseRange, _worldTransitionTime, () => SwitchWorldState(WorldState.Dystopia));
                break;
            case WorldState.Utopia:
                RadialMaskManager.AnimatePrimaryMask(_worldTransitionDist, _worldTransitionTime, () => SwitchWorldState(WorldState.Utopia));
                break;

        }
    }

    private void SwitchWorldState(WorldState state)
    {
        onWorldTransitioned?.Invoke(state);
        _currentState = state;
    }

    private void Update()
    {
        _radialMaskManager.UpdateShaderProperties();
    }
}
