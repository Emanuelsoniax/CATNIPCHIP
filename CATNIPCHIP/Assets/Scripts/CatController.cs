using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CatController : MonoBehaviour
{
    private NavMeshAgent agent;
    [HideInInspector]
    public Animator animator;

    public event Action OnDestinationReached;

    [Header("Movement Settings")]
    private float jumpHeight = 2;
    private float jumpDuration = 1;

    public float movementSpeed = 0;


    private void OnValidate()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
    }

    public void Move()
    {
        if (agent.hasPath)
        {
            var dir = (agent.steeringTarget - animator.rootPosition).normalized;
            var animationDirection = transform.InverseTransformDirection(dir);
            var isFacingMoveDirection = Vector3.Dot(dir, transform.forward) > 0.5f;


            animator.SetFloat("VelocityX", animationDirection.x);
            animator.SetFloat("VelocityZ", isFacingMoveDirection ? movementSpeed/10 : 0, 0.5f, Time.deltaTime);

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 45 * Time.deltaTime);

            if (Vector3.Distance(transform.position, agent.destination) < agent.radius / 2f)
            {
                agent.ResetPath();
            }

            float mag = dir.magnitude;
            float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
            if (mag > agent.radius / 2f)
            {
                transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);
            }
        }
        else
        {
            OnDestinationReached.Invoke();
            animator.SetFloat("VelocityX", 0, 0.25f, Time.deltaTime);
            animator.SetFloat("VelocityZ", 0, 0.25f, Time.deltaTime);
        }
    }

    private IEnumerator DoJump(Waypoint waypoint)
    {
        agent.enabled = false;
        Vector3 startPos = transform.position;
        Vector3 endPos = waypoint.Position;

        //setup for jumping
        while (true)
        {
            var dir = (endPos - startPos).normalized;
            animator.SetBool("isPivoting", true);

            float angleToMove = Vector3.SignedAngle(transform.forward, (endPos - transform.position), Vector3.up);
            animator.SetFloat("Angle", angleToMove);

            var isRotated = Vector3.Dot(dir, transform.forward) >= 0.8f;
            if (isRotated)
            {
                animator.SetBool("isPivoting", false);
                break;
            }
            yield return null;
        }

        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(.7f);


        float normalizedTime = 0.0f;

        while (normalizedTime < 1.0f)
        {
            float yOffset = jumpHeight * (normalizedTime - normalizedTime * normalizedTime);
            transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / jumpDuration;
            yield return null;
        }

        yield return null;
        agent.enabled = true;
        OnDestinationReached.Invoke();
        yield return null;

    }

    public void SetDestination(WaypointManager waypointManager)
    {
        switch (waypointManager.CurrentWaypoint.waypointType)
        {
            case Waypoint.WaypointType.JumpPoint:
                StartCoroutine(DoJump(waypointManager.NextWaypoint));
                break;
            default:
                agent.destination = waypointManager.NextWaypoint.Position;
                break;

        }
    }

    private void OnDrawGizmos()
    {
        if (agent.hasPath)
        {
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.red);
            }
        }
    }

}
