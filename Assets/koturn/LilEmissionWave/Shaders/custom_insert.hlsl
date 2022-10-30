float3 getEmissionPos(float3 positionOS)
{
#if defined(_WAVEPOSSPACE_OBJECT)
    return positionOS;
#elif defined(_WAVEPOSSPACE_OBJECT)
    return mul((float3x3)unity_ObjectToWorld, positionOS)
        / float3(
            length(unity_ObjectToWorld._m00_m10_m20),
            length(unity_ObjectToWorld._m01_m11_m21),
            length(unity_ObjectToWorld._m02_m12_m22));
#else
    if (_WavePosSpace == 0) {
        return positionOS;
    } else {
        return mul((float3x3)unity_ObjectToWorld, positionOS)
            / float3(
                length(unity_ObjectToWorld._m00_m10_m20),
                length(unity_ObjectToWorld._m01_m11_m21),
                length(unity_ObjectToWorld._m02_m12_m22));
    }
#endif
}


float pickupPosition(float3 pos)
{
#if defined(_WAVEAXIS_X)
    return pos.x;
#elif defined(_WAVEAXIS_Y)
    return pos.y;
#elif defined(_WAVEAXIS_Z)
    return pos.z;
#elif defined(_WAVEAXIS_FREE)
    float3 s3, c3;
    sincos(_WaveAxisAngles, s3, c3);
    pos.yz = mul(float2x2(c3.x, -s3.x, s3.x, c3.x), pos.yz);
    pos.zx = mul(float2x2(c3.y, -s3.y, s3.y, c3.y), pos.zx);
    pos.xy = mul(float2x2(c3.z, -s3.z, s3.z, c3.z), pos.xy);
    return pos.y;
#else
    if (_WaveAxis == 3) {
        float3 s3, c3;
        sincos(radians(_WaveAxisAngles), s3, c3);
        pos.yz = mul(float2x2(c3.x, -s3.x, s3.x, c3.x), pos.yz);
        pos.zx = mul(float2x2(c3.y, -s3.y, s3.y, c3.y), pos.zx);
        pos.xy = mul(float2x2(c3.z, -s3.z, s3.z, c3.z), pos.xy);
        return pos.y;
    } else {
        return _WaveAxis == 0 ? pos.x
            : _WaveAxis == 1 ? pos.y
            : pos.z;
    }
#endif
}
