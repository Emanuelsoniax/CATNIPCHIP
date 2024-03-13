using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BoidControllerAuthoring : MonoBehaviour
{
    public BoidAuthoring boidPrefab;
    public Transform target;
    public uint amount;
    public float spawnRadius;

    public BoidParameters parameters = new BoidParameters()
    {
        maxVelocity = 5.0f,
        seperationDistance = 0.5f,
        alignmentDistance = 2.0f,
        cohesionDistance = 2.0f,
        seperationWeight = 1.0f,
        alignmentWeight = 1.5f,
        cohesionWeight = 1.0f,
        targetWeight = 1.0f
    };


    public class BoidControllerBaker : Baker<BoidControllerAuthoring>
    {
        public override void Bake(BoidControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BoidControllerData
            {
                target = GetEntity(authoring.target, TransformUsageFlags.Dynamic),
                amount = authoring.amount,
                boidPrefab = GetEntity(authoring.boidPrefab, TransformUsageFlags.Dynamic),
                parameters = authoring.parameters,
                spawnRadius = authoring.spawnRadius
            });
        }
    }
}
