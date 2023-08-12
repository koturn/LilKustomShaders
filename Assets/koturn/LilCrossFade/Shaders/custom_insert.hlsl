/*!
 * @brief Sample _MainTexArray with cross fading.
 * @param [in] fd  Fragment data.
 * @return Blended sampled color.
 */
float4 crossFadeSample(lilFragData fd)
{
    const float crossFadeTime = max(1.0e-5, _CrossFadeTime);
    const float oneCycleTime = _DisplayTime + crossFadeTime;

    const float t1 = fmodglsl(LIL_TIME, oneCycleTime * _NumTextures);
    const float texIdx1 = floor(t1 / oneCycleTime);
    const float texIdx2 = fmodglsl(texIdx1 + 1.0, _NumTextures);
#if !defined(LIL_PASS_FORWARD_NORMAL_INCLUDED) || !defined(LIL_FEATURE_PARALLAX) || !defined(LIL_FEATURE_POM)
    const float4 col1 = LIL_SAMPLE_2D_ARRAY(_MainTexArray, sampler_MainTex, fd.uvMain, texIdx1);
    const float4 col2 = LIL_SAMPLE_2D_ARRAY(_MainTexArray, sampler_MainTex, fd.uvMain, texIdx2);
#elif defined(SHADER_API_D3D9) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
    const float4 col1 = tex2DArraygrad(_MainTexArray, float3(fd.uvMain, texIdx1), fd.uvMain, texIdx1);
    const float4 col2 = tex2DArraygrad(_MainTexArray, float3(fd.uvMain, texIdx2), fd.uvMain, texIdx2);
#else
    const float4 col1 = _MainTexArray.SampleGrad(sampler_MainTex, float3(fd.uvMain, texIdx1), fd.ddxMain, fd.ddyMain);
    const float4 col2 = _MainTexArray.SampleGrad(sampler_MainTex, float3(fd.uvMain, texIdx2), fd.ddxMain, fd.ddyMain);
#endif

    const float t2 = fmodglsl(_Time.y, oneCycleTime);
    const float blendCoeff = saturate(t2 / crossFadeTime);

    return lerp(col1, col2, blendCoeff);
}


#include "LilOptCommonFunctions.hlsl"
#include "LilOverride.hlsl"
