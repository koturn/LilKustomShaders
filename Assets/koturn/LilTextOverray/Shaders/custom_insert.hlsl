#ifdef _ENABLE_ELAPSED_TIME_ON
#    define _EnableElapsedTime true
#endif  // _ENABLE_ELAPSED_TIME_ON
#ifdef _ENABLE_AL_TIME_OF_DAY_ON
#    define _EnableALTimeOfDay true
#endif  // _ENABLE_ELAPSED_TIME_ON
#ifdef _ENABLE_FRAMERATE_ON
#    define _EnableFramerate true
#endif  // _ENABLE_FRAMERATE_ON
#ifdef _ENABLE_WORLD_POS_ON
#    define _EnableWorldPos true
#endif  // _ENABLE_WORLD_POS_ON

#ifdef SHADER_TARGET_SURFACE_ANALYSIS
#    define AUDIOLINK_STANDARD_INDEXING
#endif  // SHADER_TARGET_SURFACE_ANALYSIS

// Mechanism to index into texture.
#ifdef AUDIOLINK_STANDARD_INDEXING
#    define AudioLinkData(xycoord) tex2Dlod(_AudioTexture, float4(uint2(xycoord) * _AudioTexture_TexelSize.xy, 0, 0))
#else
#    define AudioLinkData(xycoord) _AudioTexture[uint2(xycoord)]
#endif  // AUDIOLINK_STANDARD_INDEXING


//! Number of digits in splite sheet texture.
static const float kColumns = 10.0;
//! Uv index of Unix seconds in audio texture.
static const uint2 kGeneralvuUnixSeconds = uint2(6, 23);


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
 * @brief Tests to see if Audio Link texture is available.
 * @return True if Audio Link texture is available, otherwise false.
 */
bool AudioLinkIsAvailable()
{
#ifdef AUDIOLINK_STANDARD_INDEXING
    return _AudioTexture_TexelSize.z > 16;
#else
    int width, height;
    _AudioTexture.GetDimensions(/* out */ width, /* out */ height);
    return width > 16;
#endif  // AUDIOLINK_STANDARD_INDEXING
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
    const float value = AudioLinkDecodeDataAsSeconds(kGeneralvuUnixSeconds);
    return floor(fmodglsl(value.xxx / float3(3600.0, 60.0, 1.0), float3(24.0, 60.0, 60.0)));
}
