#if !defined(LIL_OUTLINE)
#    if defined(_TEXMODE_TEXTURE_ARRAY)
#        define sampler_MainTex sampler_MainTexArray
#    elif defined(_TEXMODE_SECOND_TEXTURE_AS_ATLAS)
SAMPLER(sampler_MainTex2);
#        define sampler_MainTex sampler_MainTex2
#    endif  // defined(_TEXMODE_TEXTURE_ARRAY)
#endif  // !defined(LIL_OUTLINE)


//! One of the `_TimeSource` value; means that time source is `LIL_TIME` (`_Time.y`).
static const uint kTimeSourceElapsedTime = 0;
//! One of the `_TimeSource` value; means that time source is `_FakeTime`.
static const uint kTimeSourceFakeTime = 1;
//! One of the `_TimeSource` value; means that time source is `_VRChatTimeEncoded1` and `_VRChatTimeEncoded2` (Use UTC).
static const uint kTimeSourceVRChatUTC = 2;



float getTime();


/*!
 * @brief Sample _MainTexArray with cross fading.
 * @param [in] fd  Fragment data.
 * @return Blended sampled color.
 */
float4 crossFadeSample(lilFragData fd)
{
    const float crossFadeTime = max(1.0e-5, _CrossFadeTime);
    const float oneCycleTime = _DisplayTime + crossFadeTime;

    const float t1 = fmodglsl(getTime(), oneCycleTime * _NumTextures);
    const float texIdx1 = floor(t1 / oneCycleTime);
    const float texIdx2 = fmodglsl(texIdx1 + 1.0, _NumTextures);
#if defined(_TEXMODE_TEXTURE_ARRAY)
#    if !defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) || !defined(LIL_FEATURE_PARALLAX) || !defined(LIL_FEATURE_POM)
    const float4 col1 = LIL_SAMPLE_2D_ARRAY(_MainTexArray, sampler_MainTex, fd.uvMain, texIdx1);
    const float4 col2 = LIL_SAMPLE_2D_ARRAY(_MainTexArray, sampler_MainTex, fd.uvMain, texIdx2);
#    elif defined(SHADER_API_D3D9) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
    const float4 col1 = tex2DArraygrad(_MainTexArray, float3(fd.uvMain, texIdx1), fd.uvMain, texIdx1);
    const float4 col2 = tex2DArraygrad(_MainTexArray, float3(fd.uvMain, texIdx2), fd.uvMain, texIdx2);
#    else
    const float4 col1 = _MainTexArray.SampleGrad(sampler_MainTex, float3(fd.uvMain, texIdx1), fd.ddxMain, fd.ddyMain);
    const float4 col2 = _MainTexArray.SampleGrad(sampler_MainTex, float3(fd.uvMain, texIdx2), fd.ddxMain, fd.ddyMain);
#    endif  //  !defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) || !defined(LIL_FEATURE_PARALLAX) || !defined(LIL_FEATURE_POM)
#elif defined(_TEXMODE_SECOND_TEXTURE_AS_ATLAS)
    const float2 atlasUv1 = calcAtlasUv(fd.uv0, texIdx1, _AtlasRows, _AtlasCols);
    const float4 col1 = LIL_SAMPLE_2D(_MainTex2, sampler_MainTex, atlasUv1);
    const float2 atlasUv2 = calcAtlasUv(fd.uv0, texIdx2, _AtlasRows, _AtlasCols);
    const float4 col2 = LIL_SAMPLE_2D(_MainTex2, sampler_MainTex, atlasUv2);
#elif defined(_TEXMODE_MAIN_TEXTURE_AS_ATLAS)
    const float2 atlasUv1 = calcAtlasUv(fd.uv0, texIdx1, _AtlasRows, _AtlasCols);
    const float4 col1 = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, atlasUv1);
    const float2 atlasUv2 = calcAtlasUv(fd.uv0, texIdx2, _AtlasRows, _AtlasCols);
    const float4 col2 = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, atlasUv2);
#else
    float4 col1;
    if (texIdx1 < 2.0) {
        if (texIdx1 < 1.0) {
            col1 = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, fd.uvMain);
        } else {
            col1 = LIL_SAMPLE_2D(_MainTex2, sampler_MainTex, fd.uvMain);
        }
    } else {
        if (texIdx1 < 3.0) {
            col1 = LIL_SAMPLE_2D(_MainTex3, sampler_MainTex, fd.uvMain);
        } else {
            col1 = LIL_SAMPLE_2D(_MainTex4, sampler_MainTex, fd.uvMain);
        }
    }

    float4 col2;
    if (texIdx2 < 2.0) {
        if (texIdx2 < 1.0) {
            col2 = LIL_SAMPLE_2D(_MainTex, sampler_MainTex, fd.uvMain);
        } else {
            col2 = LIL_SAMPLE_2D(_MainTex2, sampler_MainTex, fd.uvMain);
        }
    } else {
        if (texIdx2 < 3.0) {
            col2 = LIL_SAMPLE_2D(_MainTex3, sampler_MainTex, fd.uvMain);
        } else {
            col2 = LIL_SAMPLE_2D(_MainTex4, sampler_MainTex, fd.uvMain);
        }
    }
#endif  // defined(_TEXMODE_TEXTURE_ARRAY)
    const float t2 = fmodglsl(_Time.y, oneCycleTime);
    const float blendCoeff = saturate(t2 / crossFadeTime);

    return lerp(col1, col2, blendCoeff);
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
