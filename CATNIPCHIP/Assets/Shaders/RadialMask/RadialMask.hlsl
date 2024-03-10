#ifndef RADIALMASK_INCLUDED
#define RADIALMASK_INCLUDED

int _RadialMasksAmount;
float4 _RadialMasks[8];

void GetClosestRadialMask_float(float3 position, out float4 radialMask, out float distToSphere, out float distToCenter)
{
    float bestDist = 1e30f;
    for (int i = 0; i < _RadialMasksAmount; i++)
    {
        float distCenter = length(position - _RadialMasks[i].xyz);
        float dist = distCenter - _RadialMasks[i].w;
        if (dist < bestDist)
        {
            bestDist = dist;
            distToCenter = distCenter;
            radialMask = _RadialMasks[i];
        }
    }
    
    distToSphere = bestDist;
}

void GetClosestRadialMask_half(float3 position, out float4 radialMask, out float distToSphere, out float distToCenter)
{
    GetClosestRadialMask_float(position, radialMask, distToSphere, distToCenter);

}

#endif