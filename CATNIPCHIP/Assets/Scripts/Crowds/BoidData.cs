using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BoidData : IComponentData
{
    public float3 velocity;
}
