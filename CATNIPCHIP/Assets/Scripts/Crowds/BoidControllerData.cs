using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BoidControllerData : IComponentData
{
    public Entity target;
    public Entity boidPrefab;
    public uint amount;
    public BoidParameters parameters;
    public float spawnRadius;
}


[Serializable]
public struct BoidParameters
{
    public float maxVelocity;

    [Header("Distances")]
    public float seperationDistance;
    public float alignmentDistance;
    public float cohesionDistance;
    public float obstacleAvoidanceDistance;

    [Header("Weights")]
    public float seperationWeight;
    public float alignmentWeight;
    public float cohesionWeight;
    public float targetWeight;
    public float obstacleAvoidanceWeight;

    public float MaxDistance => math.max(alignmentDistance, math.max(cohesionDistance, seperationDistance));
}