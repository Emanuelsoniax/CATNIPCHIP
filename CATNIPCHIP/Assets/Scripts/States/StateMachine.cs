using UnityEngine;

public class StateMachine
{
    [SerializeField]
    public Animator animator;

    [HideInInspector]
    public BaseState currentBehaviorState;

    public void OnStart()
    {
        currentBehaviorState?.OnStart();
    }

    public void OnUpdate()
    {
        currentBehaviorState?.OnUpdate();
    }

    public void SwitchState(BaseState state)
    {
        if(currentBehaviorState !=null)
        {
            currentBehaviorState.OnEnd();
        }

        currentBehaviorState = state;

#if UNITY_EDITOR
        Debug.Log("Switched State to " + state);
#endif

        currentBehaviorState?.OnStart();
    }

    public void UpdateState(BaseState state)
    {
        Debug.Log("hallo");
        SwitchState(state);
    }
}
