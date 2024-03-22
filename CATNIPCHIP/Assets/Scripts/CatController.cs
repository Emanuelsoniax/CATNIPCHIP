using UnityEngine;
using UnityEngine.AI;

public class CatController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private void OnValidate()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        animator.SetBool("Move", agent.hasPath);

        if (agent.hasPath)
        {
            var dir = (agent.steeringTarget - animator.rootPosition).normalized;
            var animationDirection = transform.InverseTransformDirection(dir);
            var isFacingMoveDirection = Vector3.Dot(dir, transform.forward) > 0.5f;

            //animationDirection = animationDirection * 10;

            animator.SetFloat("VelocityX", animationDirection.x);
            animator.SetFloat("VelocityZ", isFacingMoveDirection ? animationDirection.z : 0, 0.5f, Time.deltaTime);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 45 * Time.deltaTime);

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
            animator.SetFloat("VelocityX", 0);
            animator.SetFloat("VelocityZ", 0);
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var isHit = Physics.Raycast(ray, out RaycastHit hit, 20);
            if (isHit)
            {
                agent.destination = hit.point;
            }
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
