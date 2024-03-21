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

    private Vector2 velocity = Vector2.zero;
    private Vector2 smoothDeltaPosition = Vector2.zero;

    public override void OnEnd()
    {
        base.OnEnd();
        cat.GetComponent<NavMeshAgent>().speed = baseSpeed;
        cat.animator.SetBool("Move", false);
    }

    public override void OnStart()
    {
        base.OnStart();
        cat.animator.SetBool("Move", true);
        baseSpeed = agent.speed;
        agent.speed = movementSpeed;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - cat.animator.rootPosition;
        worldDeltaPosition.y = 0;

        float dx = Vector3.Dot(cat.transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(cat.transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = smoothDeltaPosition / Time.deltaTime;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance / agent.stoppingDistance);
        }

        cat.animator.SetFloat("VelocityX", 0) ;
        cat.animator.SetFloat("VelocityZ", agent.speed);

        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > agent.radius / 2f)
        {
            cat.transform.position = Vector3.Lerp(cat.animator.rootPosition, agent.nextPosition, smooth);
        }

    }

}
