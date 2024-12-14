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
#    define BRANCH
#else
#    define BRANCH UNITY_BRANCH
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


/*!
 * @brief Get emission position.
 * @param [in] positionOS  Object space position.
 * @return Emisssion position.
 */
float3 getEmissionPos(float3 positionOS)
{
    BRANCH
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


/*!
 * @brief Extract emission wave factor position.
 * @param [in] pos  Emission position.
 * @return Emisssion wave factor position.
 */
float pickupPosition(float3 pos)
{
    BRANCH
    if (_WaveAxis == kWaveAxisFree) {
        float3 s3, c3;
        sincos(_WaveAxisAngles, s3, c3);
        pos.yz = float2(
            pos.y * c3.x - pos.z * s3.x,
            pos.y * s3.x + pos.z * c3.x);
        pos.zx = float2(
            pos.z * c3.y - pos.x * s3.y,
            pos.z * s3.y + pos.x * c3.y);
        pos.xy = float2(
            pos.x * c3.z - pos.y * s3.z,
            pos.x * s3.z + pos.y * c3.z);

        return pos.y;
    } else {
        return _WaveAxis == kWaveAxisX ? pos.x
            : _WaveAxis == kWaveAxisY ? pos.y
            : pos.z;
    }
}


/*!
 * @brief Get cross faded color.
 * @param [in] crossFadeIdx1  Texture index 1.
 * @param [in] crossFadeIdx2  Texture index 2.
 * @param [in] blendCoeff  Blend coefficient.
 * @param [in] fd  Fragment data.
 * @return Blended color.
 */
float4 colorShiftGetTint(float crossFadeIdx1, float crossFadeIdx2, float blendCoeff)
{
    float4 colors[4] = {_Color, _Color2, _Color3, _Color4};
    const float4 col1 = colors[(int)crossFadeIdx1];
    const float4 col2 = colors[(int)crossFadeIdx2];
    return lerp(col1, col2, blendCoeff);
}


/*!
 * @brief Get cross faded emission wave color.
 * @param [in] crossFadeIdx1  Texture index 1.
 * @param [in] crossFadeIdx2  Texture index 2.
 * @param [in] blendCoeff  Blend coefficient.
 * @param [in] fd  Fragment data.
 * @return Blended emission wave color.
 */
float4 colorShiftGetEmissionWaveColor(float crossFadeIdx1, float crossFadeIdx2, float blendCoeff)
{
    float4 colors[4] = {_Color, _Color2, _Color3, _Color4};
    const float4 col1 = colors[(int)crossFadeIdx1];
    const float4 col2 = colors[(int)crossFadeIdx2];
    return lerp(col1, col2, blendCoeff);
}


/*!
 * @brief Calculate emission color.
 * @param [in] emissionColor  Base emission color.
 * @param [in] alpha  Alpha value.
 * @return Calculated emission color.
 */
float3 calcEmissionColor(float3 emissionColor, float alpha)
{
#if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
    return emissionColor * alpha;
#else
    return emissionColor;
#endif  // LIL_RENDER == 2 && !defined(LIL_REFRACTION)
}


#include "lil_opt_common_functions.hlsl"
#include "lil_override.hlsl"
