using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Idle", menuName = "States/Idle", order = 1)]
public class IdleState : BaseState
{
    public string animationTrigger;
    public int idleAnimationIndex;

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override void OnStart()
    {
        base.OnStart();
        cat.controller.animator.SetFloat("IdleIndex", idleAnimationIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }



}
