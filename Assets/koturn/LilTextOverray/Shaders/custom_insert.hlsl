#ifdef LIL_MULTI
#    ifdef _ENABLEELAPSEDTIME_ON
#        define _EnableElapsedTime true
#    else
#        define _EnableElapsedTime false
#    endif  // _ENABLEELAPSEDTIME_ON
#    ifdef _ENABLEALTIMEOFDAY_ON
#        define _EnableALTimeOfDay true
#    else
#        define _EnableALTimeOfDay false
#    endif  // _ENABLEALTIMEOFDAY_ON
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
//! Constant for _ALTimeOfDayKind, which means show local time.
static const int kALTimeOfDayKindLocal = 0;
//! Constant for _ALTimeOfDayKind, which means show UTC time.
static const int kALTimeOfDayKindUtc = 1;


// _AudioTexture is declared in lil_common_input.hlsl.
#ifndef LIL_FEATURE_AUDIOLINK
TEXTURE2D_FLOAT(_AudioTexture);
float4 _AudioTexture_TexelSize;
#endif  // LIL_FEATURE_AUDIOLINK


/*!
 * @brief Check if displaying Elapsed Time is enabled or not.
 * @return True if displaying Elapsed Time is enabled, otherwise false.
 */
bool isElapsedTimeEnabled()
{
    return _EnableElapsedTime;
}


/*!
 * @brief Check if displaying Times of day of AudioLink is enabled or not.
 * @return True if displaying Times of day of AudioLink is enabled, otherwise false.
 */
bool isALTimeOfDayEnabled()
{
    return _EnableALTimeOfDay;
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
 * @brief Tests to see if Audio Link texture is available.
 * @return True if Audio Link texture is available, otherwise false.
 */
bool AudioLinkIsAvailable()
{
#ifdef LIL_LWTEX
    return _AudioTexture_TexelSize.z > 16;
#else
    int width, height;
    _AudioTexture.GetDimensions(/* out */ width, /* out */ height);
    return width > 16;
#endif  // LIL_LWTEX
}


/*!
 * @brief Get AudioLink data.
 * @param [in] xycoord  XY coordinate in _AudioTexture.
 * @return AudioLink data.
 */
float4 AudioLinkData(uint2 xycoord)
{
#ifdef LIL_LWTEX
    // Assume that this function is only used in fragment shader.
    return LIL_SAMPLE_2D(_AudioTexture, sampler_linear_clamp, xycoord * _AudioTexture_TexelSize.xy);

    // If this function also used in vertex shader, use following code.
    // return LIL_SAMPLE_2D_LOD(_AudioTexture, sampler_linear_clamp, xycoord * _AudioTexture_TexelSize.xy, 0);
#else
    return _AudioTexture[xycoord];
#endif  // LIL_LWTEX
}


/*!
 * @brief Get version of audiolink present in the world., 0 if no audiolink is present
 * @return Version of audiolink present in the world. 0.0 means that no audiolink is present.
 */
float AudioLinkGetVersion()
{
    int2 dims;

#ifdef LIL_LWTEX
    dims = _AudioTexture_TexelSize.zw;
#else
    _AudioTexture.GetDimensions(dims.x, dims.y);
#endif  // LIL_LWTEX

    return dims.x >= 128 ? AudioLinkData(kGeneralvu).x
        : dims.x > 16 ? 1.0
        : 0.0;
}


/*!
 * @brief Extra utility functions for time.
 * @param [in] indexloc  Location index vector
 * @return Decoded data.
 */
uint AudioLinkDecodeDataAsUInt(uint2 indexloc)
{
    return dot(AudioLinkData(indexloc), uint4(1, 1024, 1048576, 1073741824));
}


/*!
 * @brief Decode audio link data as seconds of the time of day.
 *
 * This will truncate time to every 134,217.728 seconds (~1.5 days of an instance being up)
 * to prevent floating point aliasing.
 * If your code will alias sooner, you will need to use a different function.
 * It should be safe to use this on all times.
 *
 * @return Decoded data of time of day in seconds..
 */
float AudioLinkDecodeDataAsSeconds(uint2 indexloc)
{
    const uint time = AudioLinkDecodeDataAsUInt(indexloc) & 0x07ffffff;
    // Can't just divide by float. Bug in Unity's HLSL compiler.
    return float(time / 1000) + float(time % 1000) / 1000.0;
}


/*!
 * @brief Get time of day.
 * @return float3(hour, minute, second).
 */
float3 AudioLinkGetTimeOfDay()
{
    uint2 indexloc;
    float offsetSeconds;
    if (_ALTimeOfDayKind == kALTimeOfDayKindLocal
        || (_EnableALTimeOfDayUtcFallback && AudioLinkGetVersion() < 2.08)) {
        indexloc = kGeneralvuLocalTime;
        offsetSeconds = _ALTimeOfDayLocalTimeOffsetSeconds;
    } else {
        indexloc = kGeneralvuUnixSeconds;
        offsetSeconds = _ALTimeOfDayUtcOffsetSeconds;
    }

    const float seconds = AudioLinkDecodeDataAsSeconds(indexloc) + offsetSeconds;
    return floor(fmodglsl(seconds.xxx / float3(3600.0, 60.0, 1.0), float3(24.0, 60.0, 60.0)));
}


#include "../../LilKommon/Shaders/OptLilCommonFunctions.hlsl"
