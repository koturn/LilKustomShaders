#ifdef LIL_MULTI
#    ifdef _ENABLEELAPSEDTIME_ON
#        define _EnableElapsedTime true
#    else
#        define _EnableElapsedTime false
#    endif  // _ENABLEELAPSEDTIME_ON
#    ifdef _ENABLEVRCHATTIMEOFDAY_ON
#        define _EnableVRChatTimeOfDay true
#    else
#        define _EnableVRChatTimeOfDay false
#    endif  // _ENABLEVRCHATTIMEOFDAY_ON
#    ifdef _ENABLEFRAMERATE_ON
#        define _EnableFramerate true
#    else
#        define _EnableFramerate false
#    endif  // _ENABLEFRAMERATE_ON
#    ifdef _ENABLEWORLDPOS_ON
#        define _EnableWorldPos true
#    else
#        define _EnableWorldPos false
#    endif  // _ENABLEWORLDPOS_ON
#endif  // LIL_MULTI


//! Number of digits in splite sheet texture.
static const float kColumns = 10.0;
//! Uv index of version info in audio texture.
static const uint2 kGeneralvu = uint2(0, 22);
//! Uv index of local time in audio texture.
static const uint2 kGeneralvuLocalTime = uint2(3, 22);
//! Uv index of Unix seconds in audio texture (Supported since 0.2.8).
static const uint2 kGeneralvuUnixSeconds = uint2(6, 23);
//! Constant for _VRChatTimeOfDayKind, which means show local time.
static const int kVRChatTimeOfDayKindLocal = 0;
//! Constant for _VRChatTimeOfDayKind, which means show UTC time.
static const int kVRChatTimeOfDayKindUtc = 1;


/*!
 * @brief Check if displaying Elapsed Time is enabled or not.
 * @return True if displaying Elapsed Time is enabled, otherwise false.
 */
bool isElapsedTimeEnabled()
{
    return _EnableElapsedTime;
}


/*!
 * @brief Check if displaying Times of day is enabled or not.
 * @return True if displaying Times of day is enabled, otherwise false.
 */
bool isVRChatTimeOfDayEnabled()
{
    return _EnableVRChatTimeOfDay;
}


/*!
 * @brief Check if displaying Framerate is enabled or not.
 * @return True if displaying Framerate is enabled, otherwise false.
 */
bool isFramerateEnabled()
{
    return _EnableFramerate;
}


/*!
 * @brief Check if displaying World Position is enabled or not.
 * @return True if displaying World Position is enabled, otherwise false.
 */
bool isWorldPosEnabled()
{
    return _EnableWorldPos;
}


/*!
 * @brief Sample from splite texture with positive value and uv coordinate.
 * @param [in] val  Value to display (Assume this value is positive)
 * @param [in] uv  UV coordinate.
 * @param [in] displayLength  Number of display digits.
 * @param [in] align  Enum value of alignment.
 * @return Sampled RGB value.
 */
float3 sampleSplite(float val, float2 uv, float displayLength, float align)
{
    const float digitsCnt = ceil(log10((max(val, 1.0) + 0.5)));
    const float digitNumTmp = displayLength * (uv.x - 1.0) + (displayLength - digitsCnt) * saturate(align - 1.0);
    const float digitNum = ceil(-digitNumTmp);
    const float digit = calcDigit(val, pow(10.0, digitNum));
    const float digitPos = frac((digit + 1e-06) / kColumns);

    const float2 uv2 = float2(uv.x * displayLength, uv.y);
    const float2 spliteUv = float2((frac(uv2.x) + kColumns * digitPos) / (kColumns + 1.0), uv2.y);
    const float4 tex = LIL_SAMPLE_2D(_SpriteTex, sampler_SpriteTex, spliteUv);
    const float alpha = 2.0 - 2.0 * tex.a;
    const float mask = saturate(digitNum) * lerp(1.0, saturate(ceil(digitNumTmp + digitsCnt)), saturate(align));
    const float colAlpha = saturate((1.0 - alpha) / fwidth(alpha)) * mask;

    return tex.rgb * colAlpha;
}


/*!
 * @brief Sample from splite texture with signed value and uv coordinate.
 * @param [in] val  Value to display.
 * @param [in] uv  UV coordinate.
 * @param [in] displayLength  Number of display digits.
 * @param [in] align  Enum value of alignment.
 * @return Sampled RGB value.
 */
float3 sampleSpliteSigned(float val, float2 uv, float displayLength, float align)
{
    displayLength += 1.0;
    if (uv.x >= (1.0 / displayLength)) {
        uv.x = remap01(1.0 / displayLength, 1.0, uv.x);
        return sampleSplite(abs(val), uv, displayLength - 1.0, align);
    } else if (val <= -1.0) {
        // [0.0, 1.0 / displayLength] -> [kColumns / (kColumns + 1.0), 1.0]
        const float2 spliteUv = float2(remap(0.0, 1.0 / displayLength, kColumns / (kColumns + 1.0), 1.0, uv.x), uv.y);
        const float4 tex = LIL_SAMPLE_2D(_SpriteTex, sampler_SpriteTex, spliteUv);
        const float alpha = 2.0 - 2.0 * tex.a;
        const float colAlpha = saturate((1.0 - alpha) / fwidth(alpha));
        return tex.rgb * colAlpha;
    } else {
        return float3(0.0, 0.0, 0.0);
    }
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
 * @brief Get time of day.
 * @return float3(hour, minute, second).
 */
float3 VRChatGetTimeOfDay()
{
    uint3 hms;
    float offsetSeconds;

    if (_VRChatTimeOfDayKind == kVRChatTimeOfDayKindLocal) {
        offsetSeconds = _VRChatTimeOfDayLocalTimeOffsetSeconds;
        hms = uint3(_VRChatTimeEncoded1, _VRChatTimeEncoded1 >> 5, _VRChatTimeEncoded1 >> 11);
    } else {
        offsetSeconds = _VRChatTimeOfDayUtcOffsetSeconds;
        hms = uint3(_VRChatTimeEncoded1 >> 17, _VRChatTimeEncoded1 >> 22, _VRChatTimeEncoded1 >> 11);
    }

    hms &= uint3(0x1f, 0x3f, 0x3f);

    const float seconds = dot((float3)hms, float3(3600.0, 60.0, 1.0)) + offsetSeconds;
    return floor(fmodglsl(seconds.xxx / float3(3600.0, 60.0, 1.0), float3(24.0, 60.0, 60.0)));
}


#include "lil_opt_common_functions.hlsl"
#include "lil_override.hlsl"
