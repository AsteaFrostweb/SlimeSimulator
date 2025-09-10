#include "MathHelpers.hlsl"







void LeaveTrail(Agent a, RWTexture2D<float4> trailMap, float4 TRAIL_INTENSITY, float deltaTime)
{
    uint width, height;
    trailMap.GetDimensions(width, height);

    uint2 pixel = uint2(floor(a.position.x), floor(a.position.y));
    pixel = clamp(pixel, uint2(0, 0), uint2(width - 1, height - 1));

    trailMap[pixel] += TRAIL_INTENSITY * deltaTime;
}


