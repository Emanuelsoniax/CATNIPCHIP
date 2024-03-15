using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Moving", menuName = "States/Moving", order = 2)]
public class MovingState : BaseState
{
    public float movementSpeed;
    private float baseSpeed;

    public override void OnEnd()
    {
        base.OnEnd();
        cat.GetComponent<NavMeshAgent>().speed = baseSpeed;
    }

    public override void OnStart()
    {
        base.OnStart();
        baseSpeed = cat.GetComponent<NavMeshAgent>().speed;
        cat.GetComponent<NavMeshAgent>().speed = movementSpeed;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
