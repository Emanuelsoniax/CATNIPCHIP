using UnityEngine;

public class StateMachine: MonoBehaviour
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
        Debug.Log("Switched State to " + state, this);
#endif

        currentBehaviorState?.OnStart();
    }

    public void UpdateState(BaseState state)
    {
        currentBehaviorState = state;
        currentBehaviorState?.OnUpdate();
    }
}
