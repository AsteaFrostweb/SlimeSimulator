#include "MathHelpers.hlsl"



// Returns the average of surrounding 8 pixels + itself, ignoring pixels below threshold
float4 AverageNearbyValues(uint2 pixel, int width, int height, RWTexture2D<float4> trailMap, float threshold)
{
    float4 sum = float4(0, 0, 0, 0);
    int count = 0;

    float4 center = trailMap[pixel];
    if (length(center.rgb) * center.a > threshold)
    {
        sum += center;
        count++;
    }

    int2 offsets[8] =
    {
        int2(-1, -1), int2(0, -1), int2(1, -1),
                         int2(-1, 0), int2(1, 0),
                         int2(-1, 1), int2(0, 1), int2(1, 1)
    };

    for (int i = 0; i < 8; i++)
    {
        int2 neighbor = int2(pixel) + offsets[i];
        if (neighbor.x >= 0 && neighbor.x < width && neighbor.y >= 0 && neighbor.y < height)
        {
            float4 n = trailMap[uint2(neighbor)];
            if (length(n.rgb) * n.a > threshold)
            {
                sum += n;
                count++;
            }
        }
    }

    return count > 0 ? sum / count : trailMap[pixel];
}

void ApplyPassiveReduction(uint2 pixelLocation, RWTexture2D<float4> trailMap, float PASSIVE_TRAIL_REDUCTION, float deltaTime)
{
    float4 pixelColour = trailMap[pixelLocation];

    pixelColour.r = MoveTowards(pixelColour.r, 0.0f, PASSIVE_TRAIL_REDUCTION * deltaTime);
    pixelColour.g = MoveTowards(pixelColour.g, 0.0f, PASSIVE_TRAIL_REDUCTION * deltaTime);
    pixelColour.b = MoveTowards(pixelColour.b, 0.0f, PASSIVE_TRAIL_REDUCTION * deltaTime);
    pixelColour.a = MoveTowards(pixelColour.a, 0.0f, PASSIVE_TRAIL_REDUCTION * deltaTime);

    trailMap[pixelLocation] = pixelColour;
}

void ApplyDiffusion(uint2 pixelLocation, int width, int height, RWTexture2D<float4> trailMap, float PASSIVE_TRAIL_DIFFUSION_FACTOR, float deltaTime, float threshold)
{
    float4 avg = AverageNearbyValues(pixelLocation, width, height, trailMap, threshold);
    trailMap[pixelLocation] = MoveTowardsFloat(trailMap[pixelLocation], avg, PASSIVE_TRAIL_DIFFUSION_FACTOR * deltaTime);
}
