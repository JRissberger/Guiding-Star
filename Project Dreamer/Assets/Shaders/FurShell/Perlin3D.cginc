
float3 fade(float3 t)
{
    return t * t * t * (t * (t * 6 - 15) + 10); // new curve
    // //
    // return t * t * (3 - 2 * t); // old curve
}
float perm(float x) { return (frac(frac(x * .2 * x * 3.1415926) / 3.1415926 + x * 2.1)); }
float GradientVec(float x, float3 p) { return dot(x, p); }
// 3D version
float Perlin3Dl(float3 p)
{
    float3 P = fmod(floor(p), 256.0);
    p -= floor(p);
    float3 f = fade(p);
    // HASH COORDINATES FOR 6 OF THE 8 CUBE CORNERS
    float A = perm(P.x) + P.y;
    float AA = perm(A) + P.z;
    float AB = perm(A + 1) + P.z;
    float B = perm(P.x + 1) + P.y;
    float BA = perm(B) + P.z;
    float BB = perm(B + 1) + P.z;
    // AND ADD BLENDED RESULTS FROM 8 CORNERS OF CUBE
    return lerp(
        lerp(lerp(GradientVec(perm(AA), p), GradientVec(perm(BA), p + float3(-1, 0, 0)), f.x),
            lerp(GradientVec(perm(AB), p + float3(0, -1, 0)),
                    GradientVec(perm(BB), p + float3(-1, -1, 0)), f.x),
            f.y),
        lerp(lerp(GradientVec(perm(AA + 1), p + float3(0, 0, -1)),
                    GradientVec(perm(BA + 1), p + float3(-1, 0, -1)), f.x),
            lerp(GradientVec(perm(AB + 1), p + float3(0, -1, -1)),
                    GradientVec(perm(BB + 1), p + float3(-1, -1, -1)), f.x),
            f.y),
        f.z);
}

float3 Perlin3DVec(float3 samplePos){
    return float3(
        Perlin3Dl(samplePos),
        Perlin3Dl(samplePos - 20), 
        Perlin3Dl(samplePos + 20)
    );
}