using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Moving", menuName = "States/Moving", order = 2)]
public class MovingState : BaseState
{
    [Range(3.5f, 7f)]
    public float movementSpeed;
    private float baseSpeed;


    public override void OnEnd()
    {
        base.OnEnd();
        cat.controller.animator.SetBool("Move", false);
    }

    public override void OnStart()
    {
        cat.controller.animator.SetBool("Move", true);
        base.OnStart();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
