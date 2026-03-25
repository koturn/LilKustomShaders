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


//! One of the `_TimeSource` value; means that time source is `LIL_TIME` (`_Time.y`).
static const uint kTimeSourceElapsedTime = 0;
//! One of the `_TimeSource` value; means that time source is `_FakeTime`.
static const uint kTimeSourceFakeTime = 1;
//! One of the `_TimeSource` value; means that time source is `_VRChatTimeEncoded1` and `_VRChatTimeEncoded2` (Use UTC).
static const uint kTimeSourceVRChatUTC = 2;
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


float3 getEmissionPos(float3 positionOS);
float pickupPosition(float3 pos);
float4 getTintColor(float colorIdx);
float4 getEmissionWaveColor(float colorIdx);
float4 colorShiftGetTint(float crossFadeIdx1, float crossFadeIdx2, float blendCoeff);
float3 calcEmissionColor(float3 emissionColor, float alpha);
float getTime();


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
 * @brief Get Tint color.
 * @param [in] colorIdx  Tint color index.
 * @return _Color, _Color2, _Color3 or _Color4.
 */
float4 getTintColor(float colorIdx)
{
    float4 color;
    if (colorIdx < 2.0) {
        if (colorIdx < 1.0) {
            color = _Color;
        } else {
            color = _Color2;
        }
    } else {
        if (colorIdx < 3.0) {
            color = _Color3;
        } else {
            color = _Color4;
        }
    }

    return color;
}


/*!
 * @brief Get emission wave color.
 * @param [in] colorIdx  Emission wave color index.
 * @return _EmissionWaveColor1, _EmissionWaveColor2, _EmissionWaveColor3 or _EmissionWaveColor4.
 */
float4 getEmissionWaveColor(float colorIdx)
{
    float4 color;
    if (colorIdx < 2.0) {
        if (colorIdx < 1.0) {
            color = _EmissionWaveColor1;
        } else {
            color = _EmissionWaveColor2;
        }
    } else {
        if (colorIdx < 3.0) {
            color = _EmissionWaveColor3;
        } else {
            color = _EmissionWaveColor4;
        }
    }

    return color;
}


/*!
 * @brief Get cross faded tint color.
 * @param [in] crossFadeIdx1  COlor index 1.
 * @param [in] crossFadeIdx2  COlor index 2.
 * @param [in] blendCoeff  Blend coefficient.
 * @return Blended color.
 */
float4 colorShiftGetTint(float crossFadeIdx1, float crossFadeIdx2, float blendCoeff)
{
    const float4 col1 = getTintColor(crossFadeIdx1);
    const float4 col2 = getTintColor(crossFadeIdx2);
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


/*!
 * @brief Get the current time in seconds, including milliseconds.
 *
 * The value of `_TimeSource` determines whether to return the time elapsed
 * since entering the world, fake time or the time elapsed since midnight UTC.
 *
 * @return The current time.
 */
float getTime()
{
    float t;

    if (_TimeSource == kTimeSourceFakeTime) {
        t = _FakeTime;
    } else if (_TimeSource == kTimeSourceVRChatUTC) {
        t = dot(
            (float4)(uint4(_VRChatTimeEncoded1, _VRChatTimeEncoded1 >> 5, _VRChatTimeEncoded1 >> 11, _VRChatTimeEncoded2) & uint4(0x1f, 0x3f, 0x3f, 0x3ff)),
            float4(3600.0, 60.0, 1.0, 0.001));
    } else {
        t = LIL_TIME;
    }

    return t;
}


#include "lil_opt_common_functions.hlsl"
#include "lil_override.hlsl"
