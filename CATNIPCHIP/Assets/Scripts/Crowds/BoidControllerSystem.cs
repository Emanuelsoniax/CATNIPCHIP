using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial class BoidControllerSystem : SystemBase
{
    private bool _spawned = false;


    [BurstCompile]
    protected override void OnUpdate()
    {
        if (_spawned)
            return;

        if (!SystemAPI.TryGetSingletonEntity<BoidControllerData>(out Entity controllerEntity))
            return;

        RefRO<BoidControllerData> controller = SystemAPI.GetComponentRO<BoidControllerData>(controllerEntity);
        RefRO<LocalTransform> target = SystemAPI.GetComponentRO<LocalTransform>(controller.ValueRO.target);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Debug.Log($"Amount of boids: {controller.ValueRO.amount}");
        for (int i = 0; i < controller.ValueRO.amount; i++)
        {
            Entity newBoid = ecb.Instantiate(controller.ValueRO.boidPrefab);
            ecb.SetComponent(newBoid, new LocalTransform
            {
                Position = target.ValueRO.Position + (Unity.Mathematics.Random.CreateFromIndex((uint)i).NextFloat3() - new float3(.5f)) * controller.ValueRO.spawnRadius,
                Scale = 1.0f
            });
        }

        ecb.Playback(EntityManager);
        _spawned = true;
    }
}
