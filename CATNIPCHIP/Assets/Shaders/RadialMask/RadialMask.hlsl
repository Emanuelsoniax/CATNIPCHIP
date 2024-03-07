#ifndef RADIALMASK_INCLUDED
#define RADIALMASK_INCLUDED

int _RadialMasksAmount;
float4 _RadialMasks[8];

void GetClosestRadialMask_float(float3 position, out float4 RadialMask)
{
    float bestDist = 1e30f;
    for (int i = 0; i < _RadialMasksAmount; i++)
    {
        float dist = length(position - _RadialMasks[i].xyz) - _RadialMasks[i].w;
        if (dist < bestDist)
        {
            bestDist = dist;
            RadialMask = _RadialMasks[i];
        }

    }
}

void GetClosestRadialMask_half(float3 position, out float4 RadialMask)
{
    GetClosestRadialMask_float(position, RadialMask);

}

#endif