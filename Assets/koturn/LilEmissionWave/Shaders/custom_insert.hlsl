#ifdef LIL_MOULTI
#    if defined(_WAVEPOSSPACE_OBJECT)
#        define _WavePosSpace 0
#    elif defined(_WAVEPOSSPACE_OBJECT)
#        define _WavePosSpace 1
#    endif  // defined(_WAVEPOSSPACE_OBJECT)
#    if defined(_WAVEAXIS_X)
#        define _WaveAxis 0
#    elif defined(_WAVEAXIS_Y)
#        define _WaveAxis 1
#    elif defined(_WAVEAXIS_Z)
#        define _WaveAxis 2
#    elif defined(_WAVEAXIS_FREE)
#        define _WaveAxis 3
#    endif
#endif  // LIL_MOULTI


float3 getEmissionPos(float3 positionOS)
{
    if (_WavePosSpace == 0) {
        return positionOS;
    } else {
        return mul((float3x3)unity_ObjectToWorld, positionOS)
            / float3(
                length(unity_ObjectToWorld._m00_m10_m20),
                length(unity_ObjectToWorld._m01_m11_m21),
                length(unity_ObjectToWorld._m02_m12_m22));
    }
}


float pickupPosition(float3 pos)
{
    if (_WaveAxis == 3) {
        float3 s3, c3;
        sincos(_WaveAxisAngles, s3, c3);
        pos.yz = mul(float2x2(c3.x, -s3.x, s3.x, c3.x), pos.yz);
        pos.zx = mul(float2x2(c3.y, -s3.y, s3.y, c3.y), pos.zx);
        pos.xy = mul(float2x2(c3.z, -s3.z, s3.z, c3.z), pos.xy);
        return pos.y;
    } else {
        return _WaveAxis == 0 ? pos.x
            : _WaveAxis == 1 ? pos.y
            : pos.z;
    }
}
