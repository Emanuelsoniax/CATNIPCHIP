using UnityEngine;

[CreateAssetMenu(fileName = "Moving", menuName = "States/Moving", order = 2)]
public class MovingState : BaseState
{
    [Range(3f, 7f)]
    public float movementSpeed;


    public override void OnEnd()
    {
        base.OnEnd();
        cat.controller.animator.SetBool("Move", false);
    }

    public override void OnStart()
    {
        base.OnStart();
        cat.controller.animator.SetBool("Move", true);
        cat.controller.movementSpeed = movementSpeed;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
