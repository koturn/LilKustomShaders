#ifdef LIL_MULTI
#    if defined(_WAVEPOSSPACE_OBJECT)
#        define _WavePosSpace kWavePosSpaceObject
#    elif defined(_WAVEPOSSPACE_WORLD)
#        define _WavePosSpace kWavePosSpaceWorld
#    endif  // defined(_WAVEPOSSPACE_OBJECT)
#    if defined(_WAVEAXIS_X)
#        define _WaveAxis kWaveAxisX
#    elif defined(_WAVEAXIS_Y)
#        define _WaveAxis kWaveAxisY
#    elif defined(_WAVEAXIS_Z)
#        define _WaveAxis kWaveAxisZ
#    elif defined(_WAVEAXIS_FREE)
#        define _WaveAxis kWaveAxisFree
#    endif
#endif  // LIL_MULTI


//! Enum value for _WavePosSpace, "Object".
static const int kWavePosSpaceObject = 0;
//! Enum value for _WavePosSpace, "World".
static const int kWavePosSpaceWorld = 1;
//! Enum value for _WaveAxis, "X".
static const int kWaveAxisX = 0;
//! Enum value for _WaveAxis, "Y".
static const int kWaveAxisY = 1;
//! Enum value for _WaveAxis, "Z".
static const int kWaveAxisZ = 2;
//! Enum value for _WaveAxis, "Free".
static const int kWaveAxisFree = 3;


float3 getEmissionPos(float3 positionOS)
{
    UNITY_BRANCH
    if (_WavePosSpace == kWavePosSpaceObject) {
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
    UNITY_BRANCH
    if (_WaveAxis == kWaveAxisFree) {
        float3 s3, c3;
        sincos(_WaveAxisAngles, s3, c3);
        pos.yz = mul(float2x2(c3.x, -s3.x, s3.x, c3.x), pos.yz);
        pos.zx = mul(float2x2(c3.y, -s3.y, s3.y, c3.y), pos.zx);
        pos.xy = mul(float2x2(c3.z, -s3.z, s3.z, c3.z), pos.xy);
        return pos.y;
    } else {
        return _WaveAxis == kWaveAxisX ? pos.x
            : _WaveAxis == kWaveAxisY ? pos.y
            : pos.z;
    }
}
