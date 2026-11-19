inline float3 fade(float3 t)
{
    return t * t * t * (t * (t * 6 - 15) + 10); // new curve
    // //
    // return t * t * (3 - 2 * t); // old curve
}
// faster noise fuction from https://www.jcgt.org/published/0009/03/02/
inline uint3 permi3(uint3 v) {
    v = v * 1664525u + 1013904223u;
    v.x += v.y*v.z; v.y += v.z*v.x; v.z += v.x*v.y;
    v ^= v >> 16u;
    v.x += v.y*v.z; v.y += v.z*v.x; v.z += v.x*v.y;
    return v; 
}

inline fixed3 safeNormFixed(float3 inVec){
	float dp3 = max (0.005, dot (inVec, inVec));
	return inVec * rsqrt (dp3);
}

inline fixed3 perm(int3 s){
    uint3 u = uint3(s.y + s.z,s.x + s.z, s.x + s.y);
    u = permi3(u);
    return safeNormFixed(float3(((int3)u)/16.));
}

// 3D version
inline fixed Perlin3Dl(float3 pos)
{
    int3 G = int3(pos);
    fixed3 p = pos - floor(pos);
    fixed3 p1 = p - 1.;
    fixed3 f = fade(p);
    int3 G1 = G + 1;
    fixed AAA = dot(perm(G), p);
    fixed AAB = dot(perm(int3(G.x, G.y, G1.z)),fixed3(p .x,p .y,p1.z));
    fixed ABA = dot(perm(int3(G.x, G1.y,G.z )),fixed3(p .x,p1.y,p .z));
    fixed ABB = dot(perm(int3(G.x, G1.y,G1.z)),fixed3(p .x,p1.y,p1.z));
    fixed BAA = dot(perm(int3(G1.x,G.y, G.z )),fixed3(p1.x,p .y,p .z));
    fixed BAB = dot(perm(int3(G1.x,G.y, G1.z)),fixed3(p1.x,p .y,p1.z));
    fixed BBA = dot(perm(int3(G1.x,G1.y,G.z )),fixed3(p1.x,p1.y,p .z));
    fixed BBB = dot(perm(G1),p1);
    return lerp(
        lerp(
            lerp(
                AAA,
                AAB,
                f.z
            ),
            lerp(
                ABA,
                ABB,
                f.z
            ),
            f.y
        ),
        lerp(
            lerp(
                BAA,
                BAB,
                f.z
            ),
            lerp(
                BBA,
                BBB,
                f.z
            ),
            f.y
        ),
        f.x
    );
}

// outputs 3d perlin noise within the normalized range [0,1]
// the factor was determined based on this thread:
// https://stackoverflow.com/questions/18261982/output-range-of-perlin-noise#18263038
inline fixed Perlin3Dln(float3 pos)
{
    return Perlin3Dl(pos) * sqrt(3.) + 0.5;
}

inline fixed3 Perlin3DVec(float3 samplePos){
    return fixed3(
        Perlin3Dl(samplePos),
        Perlin3Dl(samplePos - 20), 
        Perlin3Dl(samplePos + 20)
    ) * 2. - 1.;
}