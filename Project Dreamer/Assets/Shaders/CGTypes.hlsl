// define macros to retain shorter naming 
// of cg-buitin types

// caution: DATA TYPE CAN LOSE PRECISION GIVEN SOME
// PLATFORMS, fixed is 12 bit fixed point in CG, however, 
// HLSL min10floats is both not a fixedpoint value, 
// as well as being, depending on platform, only 10 bits.
#define fixed4 min10float4    
#define fixed3 min10float3
#define fixed2 min10float2
#define fixed  min10float
