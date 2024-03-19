using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Physics;

[BurstCompile]
public partial class BoidSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingleton(out BoidControllerData controller) || !SystemAPI.TryGetSingleton(out PhysicsWorldSingleton controllerEntity))
            return;

        RefRO<LocalTransform> target = SystemAPI.GetComponentRO<LocalTransform>(controller.target);

        EntityQuery allBoids = GetEntityQuery(typeof(BoidData), typeof(LocalTransform));
        NativeArray<Entity> entityData = allBoids.ToEntityArray(Allocator.TempJob);
        NativeArray<BoidData> boidsData = allBoids.ToComponentDataArray<BoidData>(Allocator.TempJob);
        NativeArray<LocalTransform> boidsTransforms = allBoids.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        NativeParallelMultiHashMap<int, int> hashMap = new NativeParallelMultiHashMap<int, int>(entityData.Length, Allocator.TempJob);

        Debug.Log($"Current boid count: {entityData.Length}");

        float maxRadius = controller.parameters.MaxDistance;
        float offsetRange = maxRadius / 2f;
        quaternion randomHashRotation = quaternion.Euler(
                UnityEngine.Random.Range(-360f, 360f),
                UnityEngine.Random.Range(-360f, 360f),
                UnityEngine.Random.Range(-360f, 360f)
            );

        float3 randomHashOffset = new float3(
            UnityEngine.Random.Range(-offsetRange, offsetRange),
            UnityEngine.Random.Range(-offsetRange, offsetRange),
            UnityEngine.Random.Range(-offsetRange, offsetRange)
        );
        PopulateBoidHashMapJob popHashMap = new PopulateBoidHashMapJob
        {
            hashMap = hashMap.AsParallelWriter(),
            randomOffset = randomHashOffset,
            cellSize = maxRadius,
            cellRotationVary = randomHashRotation
        };
        JobHandle popHashHandle = popHashMap.ScheduleParallel(new JobHandle());

        BoidJob boidJob = new BoidJob
        {
            entityData = entityData,
            boidsData = boidsData,
            boidsTransforms = boidsTransforms,
            deltaTime = SystemAPI.Time.DeltaTime,
            parameters = controller.parameters,
            target = target.ValueRO,
            collisionWorld = controllerEntity.CollisionWorld,
            hashMap = hashMap,
            cellSize = maxRadius,
            randomOffset = randomHashOffset,
            cellRotationVary = randomHashRotation
        };
        JobHandle boidHandle = boidJob.ScheduleParallel(popHashHandle);
        boidHandle.Complete();
        hashMap.Dispose();
    }
}

/*
 * Hash optimization based on https://github.com/BogdanCodreanu/ECS-Boids-Murmuration_Unity_2019.1
 */
[BurstCompile]
public partial struct PopulateBoidHashMapJob : IJobEntity
{
    [WriteOnly] public NativeParallelMultiHashMap<int, int>.ParallelWriter hashMap;
    [ReadOnly] public float3 randomOffset;
    [ReadOnly] public float cellSize;
    [ReadOnly] public quaternion cellRotationVary;

    [BurstCompile]
    public void Execute([EntityIndexInQuery] int index, in BoidData boidData, in LocalTransform transform)
    {
        int hash = (int)math.hash(math.floor(math.mul(cellRotationVary, transform.Position + randomOffset) / cellSize));
        hashMap.Add(hash, index);
    }
}

[BurstCompile]
public partial struct BoidJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public BoidParameters parameters;
    [ReadOnly] public LocalTransform target;
    [ReadOnly] public CollisionWorld collisionWorld;

    [ReadOnly] public NativeParallelMultiHashMap<int, int> hashMap;
    [ReadOnly] public float cellSize;
    [ReadOnly] public quaternion cellRotationVary;
    [ReadOnly] public float3 randomOffset;

    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> entityData;
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BoidData> boidsData;
    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<LocalTransform> boidsTransforms;

    [BurstCompile]
    public void Execute(Entity entity, [EntityIndexInQuery] int index, ref BoidData boidData, ref LocalTransform transform)
    {
        float3 alignmentForce = float3.zero;
        float3 seperationForce = float3.zero;
        float3 cohesionForce = float3.zero;
        float3 targetForce = math.normalize(target.Position - transform.Position);

        int alignmentCount = 0;
        int cohesionCount = 0;
        float3 cohesionCenter = float3.zero;

        float3 cellPosition = math.floor(math.mul(cellRotationVary, transform.Position + randomOffset) / cellSize);
        int hash = (int)math.hash(cellPosition);
        if (hashMap.TryGetFirstValue(hash, out int itIndex, out NativeParallelMultiHashMapIterator<int> iterator))
        {
            do
            {
                if (entityData[itIndex] == entity)
                    continue;

                float3 otherVelocity = boidsData[itIndex].velocity;
                LocalTransform otherTransform = boidsTransforms[itIndex];

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
            } while (hashMap.TryGetNextValue(out itIndex, ref iterator));
        }

        // OLD CODE, left for comparison
        //for (int i = 0; i < boidsData.Length; i++)
        //{
        //    if (entityData[i] == entity)
        //        continue;

        //    float3 otherVelocity = boidsData[i].velocity;
        //    LocalTransform otherTransform = boidsTransforms[i];

        //    float3 direction = transform.Position - otherTransform.Position;
        //    float dist = math.length(direction);

        //    // Seperation
        //    if (dist < parameters.seperationDistance)
        //    {
        //        seperationForce += math.normalize(direction) / (dist < .001f ? .001f : dist);
        //    }

        //    // Alignment
        //    if (dist < parameters.alignmentDistance)
        //    {
        //        alignmentForce += otherVelocity;
        //        alignmentCount++;
        //    }

        //    // Cohesion
        //    if (dist < parameters.cohesionDistance)
        //    {
        //        cohesionCenter += otherTransform.Position;
        //        cohesionCount++;
        //    }
        //}

        if (cohesionCount > 0)
            cohesionForce = math.normalize(cohesionCenter / cohesionCount - transform.Position);

        if (alignmentCount > 0)
            alignmentForce /= alignmentCount;

        // Apply weights
        alignmentForce *= parameters.alignmentWeight;
        cohesionForce *= parameters.cohesionWeight;
        seperationForce *= parameters.seperationWeight;
        targetForce *= parameters.targetWeight;

        // Apply swarm forces
        boidData.velocity += (alignmentForce + cohesionForce + seperationForce + targetForce) * deltaTime;

        // Check for obstacles
        NativeList<DistanceHit> hits = new NativeList<DistanceHit>(0, Allocator.TempJob);
        if (collisionWorld.OverlapSphere(transform.Position, parameters.obstacleAvoidanceDistance, ref hits, CollisionFilter.Default))
        {
            for (int i = 0; i < hits.Length; i++)
            {
                DistanceHit hit = hits[i];
                float3 dirToSurface = math.normalize(transform.Position - hit.Position);
                if (math.dot(dirToSurface, hit.SurfaceNormal) < 0) dirToSurface = -dirToSurface;

                boidData.velocity += dirToSurface / (hit.Fraction < .001f ? .001f : hit.Fraction) * parameters.obstacleAvoidanceWeight * deltaTime;
            }
        }
        hits.Dispose();

        // Normalize
        if (math.length(boidData.velocity) > parameters.maxVelocity)
            boidData.velocity = math.normalize(boidData.velocity) * parameters.maxVelocity;

        // Add to position
        transform.Position += boidData.velocity * deltaTime;

        if (!math.all(boidData.velocity == float3.zero))
            transform.Rotation = quaternion.LookRotationSafe(boidData.velocity, new float3(0, 1, 0));
    }
}
