inline half bayer2x2(float2 pixelPosition){
    return pixelPosition.x * .5 + frac(pixelPosition.y + pixelPosition.x);
}

inline half bayer4x4(float2 pixelPosition){
    return bayer2x2(frac(pixelPosition * 2.)) * .75 + bayer2x2(pixelPosition) * .25;
}