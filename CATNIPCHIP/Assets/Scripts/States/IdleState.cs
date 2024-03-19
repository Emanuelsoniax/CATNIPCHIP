using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "States/Idle", order = 1)]
public class IdleState : BaseState
{
    public string animationTrigger;
    public Animation idleAnimation;

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override void OnStart()
    {
        base.OnStart();
        cat.animator?.SetTrigger(animationTrigger);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }



}
