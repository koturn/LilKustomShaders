#ifdef _ENABLE_ELAPSED_TIME_ON
#    define _EnableElapsedTime true
#endif  // _ENABLE_ELAPSED_TIME_ON
#ifdef _ENABLE_FRAMERATE_ON
#    define _EnableFramerate true
#endif  // _ENABLE_FRAMERATE_ON
#ifdef _ENABLE_WORLD_POS_ON
#    define _EnableWorldPos true
#endif  // _ENABLE_WORLD_POS_ON


//! Number of digits in splite sheet texture.
static const float kColumns = 10.0;


/*!
 * @brief Check if displaying Elapsed Time is enabled or not.
 * @return True if displaying Elapsed Time is enabled, otherwise false.
 */
bool isElapsedTimeEnabled()
{
    return _EnableElapsedTime;
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
