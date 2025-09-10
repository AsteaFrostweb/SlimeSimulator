#ifndef MATHHELPERS_INCLUDED
#define MATHHELPERS_INCLUDED

float Min(float a, float b)
{
    return a < b ? a : b;
}
float Max(float a, float b)
{
    return a > b ? a : b;
}

float MoveTowards(float current, float destination, float maxStep)
{
    float delta = destination - current;
    if (abs(delta) <= maxStep)
        return destination;
    return current + sign(delta) * maxStep;
}
float4 MoveTowardsFloat(float4 current, float4 destination, float maxStep)
{
    float4 diff = destination - current;
    float maxDiff = max(abs(diff.r), max(abs(diff.g), max(abs(diff.b), abs(diff.a))));
    if (maxDiff <= maxStep)
        return destination;
    return current + normalize(diff) * maxStep;
}
// Rotate a 2D vector by angle (radians)
float2 RotateVector(float2 v, float angle)
{
    return float2(
        v.x * cos(angle) - v.y * sin(angle),
        v.x * sin(angle) + v.y * cos(angle)
    );
}

float Rand(uint seed)
{
    seed = (seed ^ 61) ^ (seed >> 16);
    seed *= 9;
    seed = seed ^ (seed >> 4);
    seed *= 0x27d4eb2d;
    seed = seed ^ (seed >> 15);
    return frac(seed / 4294967296.0); // normalize to [0,1)
}
#endif
