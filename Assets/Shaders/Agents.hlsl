#include "MathHelpers.hlsl"

struct Agent
{
                     //BYTES
    float2 position; // 8
    float angle; // 4 = 12
    float speed; // 4 = 16
    float lifetime; // 4 = 20
    int alive; // 4 = 24
    
    float2 padding;
};
uint2 GetPixelPostion(Agent a, RWTexture2D<float4> trailMap)
{
    uint width, height;
    trailMap.GetDimensions(width, height);

    uint2 pixel = uint2(floor(a.position.x), floor(a.position.y));
    pixel = clamp(pixel, uint2(0, 0), uint2(width - 1, height - 1));
    
    return pixel;
}

// Returns the "brightness" of a pixel (length of RGB)
float SampleTrail(RWTexture2D<float4> trailMap, float2 position, uint width, uint height)
{
    // Clamp position to texture bounds
    float2 clampedPos = clamp(position, float2(0, 0), float2(width - 1, height - 1));
    return length(trailMap[uint2(clampedPos)].rgb);
}

// Compute sensor offsets based on agent angle + sensor angle offset
float2 GetSensorOffset(float sensorDistance, float sensorAngleOffset, float agentAngle)
{
    float angle = agentAngle + sensorAngleOffset; // add offset
    return float2(cos(angle), sin(angle)) * sensorDistance;
}

// Returns the three sensor positions
void GetSensorPositions(Agent a, float sensorDistance, float sensorAngleOffset,
                        out float2 fPos, out float2 lPos, out float2 rPos)
{
    fPos = a.position + GetSensorOffset(sensorDistance, 0.0, a.angle); // forward
    lPos = a.position + GetSensorOffset(sensorDistance, -sensorAngleOffset, a.angle); // left
    rPos = a.position + GetSensorOffset(sensorDistance, sensorAngleOffset, a.angle); // right
}


// Adjust agent's angle based on sensor readings
float GetSensorAngleDelta(inout Agent a, float forwardValue, float leftValue, float rightValue, float turnSpeed, float deltaTime)
{
    if (leftValue > forwardValue && leftValue > rightValue)
        return -turnSpeed;
    else if (rightValue > forwardValue && rightValue > leftValue)
        return turnSpeed;
    
    return 0.0f;
}

void BounceAgent(inout Agent a, uint width, uint height)
{
    // X-axis bounce
    if (a.position.x < 0)
    {
        a.position.x = 0;
        a.angle = 3.14159 - a.angle; // reflect horizontally
    }
    else if (a.position.x > width - 1)
    {
        a.position.x = width - 1;
        a.angle = 3.14159 - a.angle; // reflect horizontally
    }

    // Y-axis bounce
    if (a.position.y < 0)
    {
        a.position.y = 0;
        a.angle = -a.angle; // reflect vertically
    }
    else if (a.position.y > height - 1)
    {
        a.position.y = height - 1;
        a.angle = -a.angle; // reflect vertically
    }

    // Keep angle between 0 and 2π
    a.angle = fmod(a.angle, 6.28318);
    if (a.angle < 0)
        a.angle += 6.28318;
}

void DecayLifetime(inout Agent a, float deltaTime, RWTexture2D<float4> trailMap, float brightnessLifetimeScalar)
{
    if (a.lifetime <= 0)
        return;
    
    
    uint2 pixel = GetPixelPostion(a, trailMap);
    float brightness = length(trailMap[pixel].rgba) * brightnessLifetimeScalar;
    
    a.lifetime -= deltaTime / (1 + brightness);
    
    if (a.lifetime <= 0)
    {
        a.alive = 0;
    }
}

// Returns an angle delta to steer away from walls
float GetWallAvoidanceAngleDelta(Agent a, uint width, uint height, float wallAvoidanceStrength)
{
    float delta = 0.0;

    // If near left wall
    if (a.position.x < wallAvoidanceStrength)
        delta += (wallAvoidanceStrength - a.position.x) / wallAvoidanceStrength;

    // If near right wall
    if (a.position.x > width - wallAvoidanceStrength)
        delta -= (a.position.x - (width - wallAvoidanceStrength)) / wallAvoidanceStrength;

    // If near bottom wall
    if (a.position.y < wallAvoidanceStrength)
        delta += (wallAvoidanceStrength - a.position.y) / wallAvoidanceStrength;

    // If near top wall
    if (a.position.y > height - wallAvoidanceStrength)
        delta -= (a.position.y - (height - wallAvoidanceStrength)) / wallAvoidanceStrength;

    return delta;
}

void MoveAgent(inout Agent a, RWTexture2D<float4> trailMap, float deltaTime,
               float sensorDistance, float sensorAngleOffset, float turnSpeed, bool bounce, float wallAvoidanceStrength)
{
    uint width, height;
    trailMap.GetDimensions(width, height);

    // Sensor positions
    float2 fPos, lPos, rPos;
    GetSensorPositions(a, sensorDistance, sensorAngleOffset, fPos, lPos, rPos);

    // Sample trail brightness
    float forwardValue = length(trailMap[uint2(clamp(fPos, float2(0, 0), float2(width - 1, height - 1)))].rgba);
    float leftValue = length(trailMap[uint2(clamp(lPos, float2(0, 0), float2(width - 1, height - 1)))].rgba);
    float rightValue = length(trailMap[uint2(clamp(rPos, float2(0, 0), float2(width - 1, height - 1)))].rgba);

    // Sensor steering
    float angleDelta = GetSensorAngleDelta(a, forwardValue, leftValue, rightValue, turnSpeed, deltaTime);

    // Wall avoidance steering
    angleDelta += GetWallAvoidanceAngleDelta(a, width, height, wallAvoidanceStrength);

    // Apply total angle delta
    a.angle += angleDelta * deltaTime;

    

    // Move agent forward
    a.position += float2(cos(a.angle), sin(a.angle)) * a.speed * deltaTime;

  
    if (!bounce)
    {
        //edge wrap
        a.position = fmod(a.position + float2(width, height), float2(width, height));
    }
    else
    {      
        //Edge Bounce
        BounceAgent(a, width, height);
    }


}



void LeaveTrail(Agent a, RWTexture2D<float4> trailMap, float4 TRAIL_INTENSITY, float deltaTime)
{
    uint2 pixel = GetPixelPostion(a, trailMap);

    trailMap[pixel] += TRAIL_INTENSITY * deltaTime;
}


