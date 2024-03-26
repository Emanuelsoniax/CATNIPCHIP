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

    public WorldState TransitioningTo
    {
        get; private set;
    }

    public float NormalizedTime { get; private set; }

    public event Action<WorldState> onWorldTransitionStart;
    public event Action<WorldState> onWorldTransitionEnd;

    private WorldState _currentState;

    public void TemporaryTransitionToDystopia(float time) => TransitionWorldStateTemporary(time, WorldState.Dystopia);
    public void TemporaryTransitionToUtopia(float time) => TransitionWorldStateTemporary(time, WorldState.Utopia);

    public void TransitionToDystopia() => CurrentState = WorldState.Dystopia;
    public void TransitionToUtopia() => CurrentState = WorldState.Dystopia;

    public void TransitionWorldState(WorldState state)
    {
        if (!RadialMaskManager.TryGetPrimaryMask(out RadialMask mask))
        {
            Debug.LogWarning($"A primary mask is required. Make sure there is a radial mask in the first element of the masks collection.");
            return;
        }

        if (TransitioningTo == state || CurrentState == state)
        {
            Debug.LogWarning($"Already transitioning or transitioned to this world state.");
            return;
        }

        TransitioningTo = state;
        switch (state)
        {
            case WorldState.Dystopia:
                SwitchWorldState(mask.BaseRange, WorldState.Dystopia, mask);
                break;
            case WorldState.Utopia:
                SwitchWorldState(_worldTransitionDist, WorldState.Utopia, mask);
                break;
        }
    }

    public void TransitionWorldStateTemporary(float time, WorldState state)
    {
        StartCoroutine(TransitionCoroutine(state, time));
    }

    private void SwitchWorldState(float target, WorldState state, RadialMask primaryMask)
    {
        onWorldTransitionStart?.Invoke(state);
        RadialMaskManager.AnimatePrimaryMask(target, _worldTransitionTime,
            () =>
            {
                onWorldTransitionEnd?.Invoke(state);
                _currentState = state;
            },
            val => NormalizedTime = (val - primaryMask.BaseRange) / _worldTransitionDist);
    }

    private IEnumerator TransitionCoroutine(WorldState state, float time)
    {
        if (TransitioningTo != state && CurrentState != state)
        {
            TransitionWorldState(state);

            yield return new WaitForSeconds(time);

            TransitionWorldState(state == WorldState.Dystopia ? WorldState.Utopia : WorldState.Dystopia);
        }
    }

    private void Update()
    {
        _radialMaskManager.UpdateShaderProperties();
        Shader.SetGlobalFloat("_WorldTransitionTime", NormalizedTime);
    }
}
