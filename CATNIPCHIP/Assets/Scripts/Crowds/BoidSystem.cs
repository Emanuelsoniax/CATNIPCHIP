using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class BoidSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingletonEntity<BoidControllerData>(out Entity controllerEntity))
            return;

        RefRO<BoidControllerData> controller = SystemAPI.GetComponentRO<BoidControllerData>(controllerEntity);
        RefRO<LocalTransform> target = SystemAPI.GetComponentRO<LocalTransform>(controller.ValueRO.target);

        EntityQuery allBoids = GetEntityQuery(typeof(BoidData), typeof(LocalTransform));
        NativeArray<Entity> entityData = allBoids.ToEntityArray(Allocator.TempJob);
        NativeArray<BoidData> boidsData = allBoids.ToComponentDataArray<BoidData>(Allocator.TempJob);
        NativeArray<LocalTransform> boidsTransforms = allBoids.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        BoidJob job = new BoidJob
        {
            entityData = entityData,
            boidsData = boidsData,
            boidsTransforms = boidsTransforms,
            deltaTime = SystemAPI.Time.DeltaTime,
            parameters = controller.ValueRO.parameters,
            target = target.ValueRO
        };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct BoidJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public BoidParameters parameters;
    [ReadOnly] public LocalTransform target;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> entityData;
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BoidData> boidsData;
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<LocalTransform> boidsTransforms;

    [BurstCompile]
    public void Execute(Entity entity, ref BoidData boidData, ref LocalTransform transform)
    {
        float3 alignmentForce = float3.zero;
        float3 seperationForce = float3.zero;
        float3 cohesionForce = float3.zero;
        float3 targetForce = math.normalize(target.Position - transform.Position);

        int alignmentCount = 0;
        int cohesionCount = 0;
        float3 cohesionCenter = float3.zero;

        for (int i = 0; i < boidsData.Length; i++)
        {
            if (entityData[i] == entity)
                continue;

            float3 otherVelocity = boidsData[i].velocity;
            LocalTransform otherTransform = boidsTransforms[i];

            float3 direction = transform.Position - otherTransform.Position;
            float dist = math.length(direction);

            // Seperation
            if (dist < parameters.seperationDistance)
            {
                seperationForce += math.normalize(direction) / (dist < .001f ? .001f : dist);
            }

            // Alignment
            if (dist < parameters.alignmentDistance)
            {
                alignmentForce += otherVelocity;
                alignmentCount++;
            }

            // Cohesion
            if (dist < parameters.cohesionDistance)
            {
                cohesionCenter += otherTransform.Position;
                cohesionCount++;
            }
        }

        if (cohesionCount > 0)
            cohesionForce = math.normalize(cohesionCenter / cohesionCount - transform.Position);

        if (alignmentCount > 0)
            alignmentForce /= alignmentCount;

        // Apply weights
        alignmentForce *= parameters.alignmentWeight;
        cohesionForce *= parameters.cohesionWeight;
        seperationForce *= parameters.seperationWeight;
        targetForce *= parameters.targetWeight;

        boidData.velocity += (alignmentForce + cohesionForce + seperationForce + targetForce) * deltaTime;
        if (math.length(boidData.velocity) > parameters.maxVelocity)
            boidData.velocity = math.normalize(boidData.velocity) * parameters.maxVelocity;

        transform.Position += boidData.velocity * deltaTime;

        if (!math.all(boidData.velocity == float3.zero))
            transform.Rotation = quaternion.LookRotationSafe(boidData.velocity, new float3(0,1,0));
    }
}
