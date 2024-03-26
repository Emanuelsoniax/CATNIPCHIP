using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 Velocity { get; set; }

    public void UpdateBoid(BoidParameters parameters, Transform target, Boid[] boids)
    {
        Vector3 alignmentForce = Vector3.zero;
        Vector3 seperationForce = Vector3.zero;
        Vector3 cohesionForce = Vector3.zero;
        Vector3 targetForce = (target.position - transform.position).normalized;

        int alignmentCount = 0;
        int cohesionCount = 0;
        Vector3 cohesionCenter = Vector3.zero;

        foreach (Boid boid in boids)
        {
            Vector3 direction = transform.position - boid.transform.position;
            float dist = direction.magnitude;

            // Seperation
            if (dist < parameters.seperationDistance)
            {
                seperationForce += direction.normalized / (dist == 0 ? .0001f : dist);
            }

            // Alignment
            if (dist < parameters.alignmentDistance)
            {
                alignmentForce += boid.Velocity;
                alignmentCount++;
            }

            // Cohesion
            if (dist < parameters.cohesionDistance)
            {
                cohesionCenter += boid.transform.position;
                cohesionCount++;
            }
        }

        if (cohesionCount > 0)
            cohesionForce = (cohesionCenter / cohesionCount - transform.position).normalized;

        if (alignmentCount > 0)
            alignmentForce /= alignmentCount;

        // Apply weights
        alignmentForce *= parameters.alignmentWeight;
        cohesionForce *= parameters.cohesionWeight;
        seperationForce *= parameters.seperationWeight;
        targetForce *= parameters.targetWeight;

        Velocity += (alignmentForce + cohesionForce + seperationForce + targetForce) * Time.deltaTime;
        if (Velocity.magnitude > parameters.maxVelocity)
            Velocity = Velocity.normalized * parameters.maxVelocity;

        transform.position += Velocity * Time.deltaTime;

        if (Velocity != Vector3.zero)
            transform.forward = Velocity.normalized;
    }
}
