using UnityEngine;

public class StateMachine: MonoBehaviour
{
    public enum States { Idle, Waiting, Walking, Running, Jumping };
    public States currentState;

    [SerializeField]
    private Animator animator;

    public void OnStart()
    {
        currentState = States.Idle;
    }

    public void OnUpdate()
    {
        switch (currentState)
        {
            case States.Idle:
                IdleBehaviour();
                break;
            case States.Waiting:
                WaitingBehaviour();
                break;
            case States.Walking:
                WalkingBehaviour();
                break;
            case States.Running:
                RunningBehaviour();
                break;

            default:
                IdleBehaviour();
                break;
        }
    }

    public void SwitchState(States state)
    {
        currentState = state;
    }

    public virtual void IdleBehaviour() {}
    public virtual void WaitingBehaviour() { }
    public virtual void WalkingBehaviour() { }
    public virtual void RunningBehaviour() { }
    public virtual void JumpBehaviour() { }


}
