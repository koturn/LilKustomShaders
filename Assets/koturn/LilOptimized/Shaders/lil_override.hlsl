#ifndef LIL_OVERRIDE_INCLUDED
#define LIL_OVERRIDE_INCLUDED


#ifdef LIL_HDRP
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_VAR , posInput
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS , PositionInputs posInput
#else
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_VAR
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS
#endif  // LIL_HDRP


float3 lilTooningNoSaturateScaleOverride(float3 aascale, float3 value, float3 border, float3 blur, float3 borderRange);
float4 lilTooningNoSaturateScaleOverride(float4 aascale, float4 value, float4 border, float4 blur, float4 borderRange);
float3 lilTooningScaleOverride(float3 aascale, float3 value, float3 border, float3 blur, float3 borderRange);
float4 lilTooningScaleOverride(float4 aascale, float4 value, float4 border, float4 blur, float4 borderRange);
#if defined(LIL_FEATURE_SHADOW) && !defined(LIL_GEM)
void lilGetShadingOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp));
#endif  // defined(LIL_FEATURE_SHADOW) && !defined(LIL_GEM)
#if defined(LIL_REFRACTION) && !defined(LIL_LITE)
void lilRefractionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp));
#endif  // defined(LIL_REFRACTION) && !defined(LIL_LITE)
#if defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE)
float3 lilCalcSpecularOverride(inout lilFragData fd, float3 L, float3 specular, float attenuation LIL_SAMP_IN_FUNC(samp));
void lilReflectionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp) LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS);
#endif  // defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE)


#ifndef OVERRIDE_REFRACTION
#    define OVERRIDE_REFRACTION \
        lilRefractionOverride(fd LIL_SAMP_IN(sampler_MainTex));
#endif  // !OVERRIDE_REFRACTION
#ifndef OVERRIDE_REFLECTION
#    define OVERRIDE_REFLECTION \
         lilReflectionOverride(fd LIL_SAMP_IN(sampler_MainTex) LIL_OVERRIDE_HDRP_POSITION_INPUT_VAR);
#endif  // !OVERRIDE_REFLECTION


#if LIL_ANTIALIAS_MODE == 0
/*!
 * @brief Calculate tooning scale without saturete.
 *
 * This function is vectorized implemnetation of lilTooningNoSaturateScale() defined in lil_common_macro.hlsl.
 *
 * @param [in] aascale  AA strength.
 * @param [in] value  Value
 * @param [in] border  Border.
 * @param [in] blur  Blue.
 * @param [in] borderRange  Shadow border range.
 * @return Tooning value.
 */
float3 lilTooningNoSaturateScaleOverride(float3 aascale, float3 value, float3 border, float3 blur, float3 borderRange)
{
    const float3 borderMin = saturate(border - blur * 0.5 - borderRange);
    const float3 borderMax = saturate(border + blur * 0.5);
    return (value - borderMin) / saturate(borderMax - borderMin);
}


/*!
 * @brief Calculate tooning scale without saturete.
 *
 * This function is vectorized implemnetation of lilTooningNoSaturateScale() defined in lil_common_macro.hlsl.
 *
 * @param [in] aascale  AA strength.
 * @param [in] value  Value
 * @param [in] border  Border.
 * @param [in] blur  Blue.
 * @param [in] borderRange  Shadow border range.
 * @return Tooning value.
 */
float4 lilTooningNoSaturateScaleOverride(float4 aascale, float4 value, float4 border, float4 blur, float4 borderRange)
{
    const float4 borderMin = saturate(border - blur * 0.5 - borderRange);
    const float4 borderMax = saturate(border + blur * 0.5);
    return (value - borderMin) / saturate(borderMax - borderMin);
}
#else
/*!
 * @brief Calculate tooning scale without saturete.
 *
 * This function is vectorized implemnetation of lilTooningNoSaturateScale() defined in lil_common_macro.hlsl.
 *
 * @param [in] aascale  AA strength.
 * @param [in] value  Value
 * @param [in] border  Border.
 * @param [in] blur  Blue.
 * @param [in] borderRange  Shadow border range.
 * @return Tooning value.
 */
float3 lilTooningNoSaturateScaleOverride(float3 aascale, float3 value, float3 border, float3 blur, float3 borderRange)
{
    const float3 borderMin = saturate(border - blur * 0.5 - borderRange);
    const float3 borderMax = saturate(border + blur * 0.5);
    return (value - borderMin) / saturate(borderMax - borderMin + fwidth(value) * aascale);
}


/*!
 * @brief Calculate tooning scale without saturete.
 *
 * This function is vectorized implemnetation of lilTooningNoSaturateScale() defined in lil_common_macro.hlsl.
 *
 * @param [in] aascale  AA strength.
 * @param [in] value  Value
 * @param [in] border  Border.
 * @param [in] blur  Blue.
 * @param [in] borderRange  Shadow border range.
 * @return Tooning value.
 */
float4 lilTooningNoSaturateScaleOverride(float4 aascale, float4 value, float4 border, float4 blur, float4 borderRange)
{
    const float4 borderMin = saturate(border - blur * 0.5 - borderRange);
    const float4 borderMax = saturate(border + blur * 0.5);
    return (value - borderMin) / saturate(borderMax - borderMin + fwidth(value) * aascale);
}
#endif  // LIL_ANTIALIAS_MODE == 0


/*!
 * @brief Calculate tooning scale.
 *
 * This function is vectorized implemnetation of lilTooningScale() defined in lil_common_macro.hlsl.
 *
 * @param [in] aascale  AA strength.
 * @param [in] value  Value
 * @param [in] border  Border.
 * @param [in] blur  Blue.
 * @param [in] borderRange  Shadow border range.
 * @return Tooning value.
 * @see lilTooningNoSaturateScaleOverride
 */
float3 lilTooningScaleOverride(float3 aascale, float3 value, float3 border, float3 blur, float3 borderRange)
{
    return saturate(lilTooningNoSaturateScaleOverride(aascale, value, border, blur, borderRange));
}


/*!
 * @brief Calculate tooning scale.
 *
 * This function is vectorized implemnetation of lilTooningScale() defined in lil_common_macro.hlsl.
 *
 * @param [in] aascale  AA strength.
 * @param [in] value  Value
 * @param [in] border  Border.
 * @param [in] blur  Blue.
 * @param [in] borderRange  Shadow border range.
 * @return Tooning value.
 * @see lilTooningNoSaturateScaleOverride
 */
float4 lilTooningScaleOverride(float4 aascale, float4 value, float4 border, float4 blur, float4 borderRange)
{
    return saturate(lilTooningNoSaturateScaleOverride(aascale, value, border, blur, borderRange));
}


#if defined(LIL_FEATURE_SHADOW) && !defined(LIL_GEM) && !defined(LIL_FAKESHADOW) && !defined(LIL_BAKER)

#ifndef OVERRIDE_SHADOW
#    define OVERRIDE_SHADOW \
        lilGetShadingOverride(fd LIL_SAMP_IN(sampler_MainTex));
#endif

#ifndef LIL_LITE
/*!
 * @brief Calculate shading.
 *
 * This function is optimized implemnetation of lilGetShading() defined in lil_common_frag.hlsl.
 * Vectorize calculation.
 *
 * @param [in,out] fd  Fragment shader data.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Sampler stete (Optional argument).
 * @see lilTooningNoSaturateScaleOverride
 * @see lilTooningScaleOverride
 */
void lilGetShadingOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
{
    if (_UseShadow) {
        // Normal
        float3 N1 = fd.N;
        float3 N2 = fd.N;
#ifdef  LIL_FEATURE_SHADOW_3RD
        float3 N3 = fd.N;
#endif  // LIL_FEATURE_SHADOW_3RD
#if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
        N1 = lerp(fd.origN, fd.N, _ShadowNormalStrength);
        N2 = lerp(fd.origN, fd.N, _Shadow2ndNormalStrength);
#    ifdef  LIL_FEATURE_SHADOW_3RD
        N3 = lerp(fd.origN, fd.N, _Shadow3rdNormalStrength);
#    endif  // LIL_FEATURE_SHADOW_3RD
#endif  // defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)

        // Shade
        float4 lns = 1.0;
        lns.x = dot(fd.L, N1);
        lns.y = dot(fd.L, N2);
#ifdef LIL_FEATURE_SHADOW_3RD
        lns.z = dot(fd.L, N3);
        lns.xyz = saturate(lns.xyz * 0.5 + 0.5);
#else
        lns.xy = saturate(lns.xy * 0.5 + 0.5);
#endif  // LIL_FEATURE_SHADOW_3RD

        // Shadow
#if (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
        float calculatedShadow = saturate(fd.attenuation + distance(fd.L, fd.origL));
#    ifdef LIL_FEATURE_SHADOW_3RD
        // !!Nor effect!!
        // lns.x *= lerp(1.0, calculatedShadow, _ShadowReceive);
        // lns.y *= lerp(1.0, calculatedShadow, _Shadow2ndReceive);
        // lns.z *= lerp(1.0, calculatedShadow, _Shadow3rdReceive);
        lns.xyz *= lerp(1.0, calculatedShadow, float3(_ShadowReceive, _Shadow2ndReceive, _Shadow3rdReceive));
#    else
        // lns.x *= lerp(1.0, calculatedShadow, _ShadowReceive);
        // lns.y *= lerp(1.0, calculatedShadow, _Shadow2ndReceive);
        lns.xy *= lerp(1.0, calculatedShadow, float2(_ShadowReceive, _Shadow2ndReceive));
#    endif  // LIL_FEATURE_SHADOW_3RD
#endif  // (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)

        // Blur Scale
#ifdef LIL_FEATURE_SHADOW_3RD
        float3 shadowBlurs = float3(_ShadowBlur, _Shadow2ndBlur, _Shadow3rdBlur);
#else
        float2 shadowBlurs = float2(_ShadowBlur, _Shadow2ndBlur);
#endif

#ifdef LIL_FEATURE_ShadowBlurMask
#    ifdef _ShadowBlurMaskLOD
        float4 shadowBlurMask = LIL_SAMPLE_2D(_ShadowBlurMask, lil_sampler_linear_repeat, fd.uvMain);
        if (_ShadowBlurMaskLOD) {
            shadowBlurMask = LIL_SAMPLE_2D_GRAD(_ShadowBlurMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBlurMaskLOD), max(fd.ddyMain, _ShadowBlurMaskLOD));
        }
#    else
        const float4 shadowBlurMask = LIL_SAMPLE_2D_GRAD(_ShadowBlurMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBlurMaskLOD), max(fd.ddyMain, _ShadowBlurMaskLOD));
#    endif  // _ShadowBlurMaskLOD
#    ifdef LIL_FEATURE_SHADOW_3RD
        shadowBlurs *= shadowBlurMask.rgb;
#    else
        shadowBlurs *= shadowBlurMask.rg;
#    endif  // LIL_FEATURE_SHADOW_3RD
#endif  // LIL_FEATURE_ShadowBlurMask

        // AO Map & Toon
#ifdef LIL_FEATURE_ShadowBorderMask
#    ifdef _ShadowBorderMaskLOD
        float4 shadowBorderMask = LIL_SAMPLE_2D(_ShadowBorderMask, lil_sampler_linear_repeat, fd.uvMain);
        if (_ShadowBorderMaskLOD) {
            shadowBorderMask = LIL_SAMPLE_2D_GRAD(_ShadowBorderMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBorderMaskLOD), max(fd.ddyMain, _ShadowBorderMaskLOD));
        }
#    else
        float4 shadowBorderMask = LIL_SAMPLE_2D_GRAD(_ShadowBorderMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowBorderMaskLOD), max(fd.ddyMain, _ShadowBorderMaskLOD));
#    endif  // _ShadowBorderMaskLOD
        // !!No effect!!
        // shadowBorderMask.r = saturate(shadowBorderMask.r * _ShadowAOShift.x + _ShadowAOShift.y);
        // shadowBorderMask.g = saturate(shadowBorderMask.g * _ShadowAOShift.z + _ShadowAOShift.w);
#    ifdef LIL_FEATURE_SHADOW_3RD
        // shadowBorderMask.b = saturate(shadowBorderMask.b * _ShadowAOShift2.x + _ShadowAOShift2.y);
        shadowBorderMask.rgb = saturate(shadowBorderMask.rgb * float3(_ShadowAOShift.xz, _ShadowAOShift2.x) + float3(_ShadowAOShift.yw, _ShadowAOShift2.y));
#    else
        shadowBorderMask.rg = saturate(shadowBorderMask.rg * _ShadowAOShift.xz + _ShadowAOShift.yw);
#    endif  // LIL_FEATURE_SHADOW_3RD
        lns.xyz = _ShadowPostAO ? lns.xyz : lns.xyz * shadowBorderMask.rgb;
        lns.w = lns.x;
#    ifdef LIL_FEATURE_SHADOW_3RD
        lns = lilTooningNoSaturateScaleOverride(_AAStrength.xxxx, lns, float4(_ShadowBorder, _Shadow2ndBorder, _Shadow3rdBorder, _ShadowBorder), shadowBlurs.xyzx, float4(0.0, 0.0, 0.0, _ShadowBorderRange));
#    else
        lns.xyw = lilTooningNoSaturateScaleOverride(_AAStrength.xxx, lns.xyw, float3(_ShadowBorder, _Shadow2ndBorder, _ShadowBorder), shadowBlurs.xyx, float3(0.0, 0.0, _ShadowBorderRange));
#    endif  // LIL_FEATURE_SHADOW_3RD
        lns = _ShadowPostAO ? lns * shadowBorderMask.rgbr : lns;
        lns = saturate(lns);
#else
        lns.w = lns.x;
#    ifdef LIL_FEATURE_SHADOW_3RD
        lns = lilTooningScaleOverride(_AAStrength.xxxx, lns, float4(_ShadowBorder, _Shadow2ndBorder, _Shadow3rdBorder, _ShadowBorder), shadowBlurs.xyzx, float4(0.0, 0.0, 0.0, _ShadowBorderRange));
#    else
        lns.xyw = lilTooningScaleOverride(_AAStrength.xxx, lns.xyw, float3(_ShadowBorder, _Shadow2ndBorder, _ShadowBorder), shadowBlurs.xyx, float3(0.0, 0.0, _ShadowBorderRange));
#    endif  // LIL_FEATURE_SHADOW_3RD
#endif  // LIL_FEATURE_ShadowBorderMask

        // Force shadow on back face
        const float bfshadow = (fd.facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
#ifdef LIL_FEATURE_SHADOW_3RD
        lns *= bfshadow;
#else
        lns.xyw *= bfshadow;
#endif  // LIL_FEATURE_SHADOW_3RD
        // Copy
        fd.shadowmix = lns.x;

        // Strength
        float shadowStrength = _ShadowStrength;
#ifdef LIL_COLORSPACE_GAMMA
        shadowStrength = lilSRGBToLinear(shadowStrength);
#endif  // LIL_COLORSPACE_GAMMA
        float shadowStrengthMask = 1;
#ifdef LIL_FEATURE_ShadowStrengthMask
#    ifdef _ShadowStrengthMaskLOD
        shadowStrengthMask = LIL_SAMPLE_2D(_ShadowStrengthMask, lil_sampler_linear_repeat, fd.uvMain).r;
        if (_ShadowStrengthMaskLOD) {
            shadowStrengthMask = LIL_SAMPLE_2D_GRAD(_ShadowStrengthMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowStrengthMaskLOD), max(fd.ddyMain, _ShadowStrengthMaskLOD)).r;
        }
#    else
        shadowStrengthMask = LIL_SAMPLE_2D_GRAD(_ShadowStrengthMask, lil_sampler_linear_repeat, fd.uvMain, max(fd.ddxMain, _ShadowStrengthMaskLOD), max(fd.ddyMain, _ShadowStrengthMaskLOD)).r;
#    endif  // _ShadowStrengthMaskLOD
#endif  // LIL_FEATURE_ShadowStrengthMask
        if (_ShadowMaskType) {
            const float3 flatN = normalize(mul((float3x3)LIL_MATRIX_M, float3(0.0, 0.25, 1.0)));  // normalize(LIL_MATRIX_M._m02_m12_m22);
            float lnFlat = saturate((dot(flatN, fd.L) + _ShadowFlatBorder) / _ShadowFlatBlur);
#if (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
            lnFlat *= lerp(1.0, calculatedShadow, _ShadowReceive);
#endif  // (defined(LIL_USE_SHADOW) || defined(LIL_LIGHTMODE_SHADOWMASK)) && defined(LIL_FEATURE_RECEIVE_SHADOW)
            lns = lerp(lnFlat, lns, shadowStrengthMask);
        } else {
            shadowStrength *= shadowStrengthMask;
        }
        lns.x = lerp(1.0, lns.x, shadowStrength);

        // Shadow Colors
        float4 shadowColorTex = 0.0;
        float4 shadow2ndColorTex = 0.0;
        float4 shadow3rdColorTex = 0.0;
#ifdef LIL_FEATURE_SHADOW_LUT
        if (_ShadowColorType == 1) {
            float4 uvShadow;
            float factor;
            lilCalcLUTUV(fd.albedo, 16, 1, uvShadow, factor);
#    ifdef LIL_FEATURE_ShadowColorTex
            shadowColorTex = lilSampleLUT(uvShadow, factor, _ShadowColorTex);
#    endif  // LIL_FEATURE_ShadowColorTex
#    ifdef LIL_FEATURE_Shadow2ndColorTex
            shadow2ndColorTex = lilSampleLUT(uvShadow, factor, _Shadow2ndColorTex);
#    endif  // LIL_FEATURE_Shadow2ndColorTex
#    if defined(LIL_FEATURE_SHADOW_3RD) && defined(LIL_FEATURE_Shadow3rdColorTex)
            shadow3rdColorTex = lilSampleLUT(uvShadow, factor, _Shadow3rdColorTex);
#    endif  // defined(LIL_FEATURE_SHADOW_3RD) && defined(LIL_FEATURE_Shadow3rdColorTex)
        }
        else
#endif  // LIL_FEATURE_SHADOW_LUT
        {
#ifdef LIL_FEATURE_ShadowColorTex
            shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, samp, fd.uvMain);
#endif  // LIL_FEATURE_ShadowColorTex
#ifdef LIL_FEATURE_Shadow2ndColorTex
            shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, samp, fd.uvMain);
#endif  // LIL_FEATURE_Shadow2ndColorTex
#if defined(LIL_FEATURE_SHADOW_3RD) && defined(LIL_FEATURE_Shadow3rdColorTex)
            shadow3rdColorTex = LIL_SAMPLE_2D(_Shadow3rdColorTex, samp, fd.uvMain);
#endif  // defined(LIL_FEATURE_SHADOW_3RD) && defined(LIL_FEATURE_Shadow3rdColorTex)
        }

        // Shadow Color 1
        float3 indirectCol = lerp(fd.albedo, shadowColorTex.rgb, shadowColorTex.a) * _ShadowColor.rgb;

        // Shadow Color 2
        shadow2ndColorTex.rgb = lerp(fd.albedo, shadow2ndColorTex.rgb, shadow2ndColorTex.a) * _Shadow2ndColor.rgb;
        lns.y = _Shadow2ndColor.a - lns.y * _Shadow2ndColor.a;
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, lns.y);

#ifdef LIL_FEATURE_SHADOW_3RD
        // Shadow Color 3
        shadow3rdColorTex.rgb = lerp(fd.albedo, shadow3rdColorTex.rgb, shadow3rdColorTex.a) * _Shadow3rdColor.rgb;
        lns.z = _Shadow3rdColor.a - lns.z * _Shadow3rdColor.a;
        indirectCol = lerp(indirectCol, shadow3rdColorTex.rgb, lns.z);
#endif  // LIL_FEATURE_SHADOW_3RD

        // Multiply Main Color
        indirectCol = lerp(indirectCol, indirectCol*fd.albedo, _ShadowMainStrength);

        // Apply Light
        const float3 directCol = fd.albedo * fd.lightColor;
        indirectCol = indirectCol * fd.lightColor;

#ifndef LIL_PASS_FORWARDADD
        // Environment Light
        indirectCol = lerp(indirectCol, fd.albedo, fd.indLightColor);
#endif  // !LIL_PASS_FORWARDADD
        // Fix
        indirectCol = min(indirectCol, directCol);
        // Gradation
        indirectCol = lerp(indirectCol, directCol, lns.w * _ShadowBorderColor.rgb);

        // Mix
        fd.col.rgb = lerp(indirectCol, directCol, lns.x);
    } else {
        fd.col.rgb *= fd.lightColor;
    }
}
#else
/*!
 * @brief Calculate shading.
 *
 * This function is optimized implemnetation of lilGetShading() defined in lil_common_frag.hlsl.
 * Vectorize calculation.
 *
 * @param [in,out] fd  Fragment shader data.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Sampler stete (Optional argument).
 */
void lilGetShadingOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
{
    if (_UseShadow) {
        // Shade
        float ln1 = saturate(fd.ln * 0.5 + 0.5);

        // Toon
        float3 lns = lilTooning(
            ln1.xxx,
            float3(_ShadowBorder, _Shadow2ndBorder, _ShadowBorder),
            float3(_ShadowBlur, _Shadow2ndBlur, _ShadowBlur),
            float3(0.0, 0.0, _ShadowBorderRange));

        // Force shadow on back face
        const float bfshadow = (fd.facing < 0.0) ? 1.0 - _BackfaceForceShadow : 1.0;
        lns *= bfshadow;

        // Copy
        fd.shadowmix = ln1;

        // Shadow Color 1
        const float4 shadowColorTex = LIL_SAMPLE_2D(_ShadowColorTex, samp, fd.uvMain);
        float3 indirectCol = lerp(fd.albedo, shadowColorTex.rgb, shadowColorTex.a);
        // Shadow Color 2
        const float4 shadow2ndColorTex = LIL_SAMPLE_2D(_Shadow2ndColorTex, samp, fd.uvMain);
        indirectCol = lerp(indirectCol, shadow2ndColorTex.rgb, shadow2ndColorTex.a - ln2 * shadow2ndColorTex.a);

        // Apply Light
        const float3 directCol = fd.albedo * fd.lightColor;
        indirectCol = indirectCol * fd.lightColor;

        // Environment Light
        indirectCol = lerp(indirectCol, fd.albedo, fd.indLightColor);
        // Fix
        indirectCol = min(indirectCol, directCol);
        // Gradation
        indirectCol = lerp(indirectCol, directCol, lnB * _ShadowBorderColor.rgb);

        // Mix
        fd.col.rgb = lerp(indirectCol, directCol, ln1);
    } else {
        fd.col.rgb *= fd.lightColor;
    }
}
#endif  // !LIL_LITE
#endif  // defined(LIL_FEATURE_SHADOW) && !defined(LIL_GEM) && !defined(LIL_FAKESHADOW) && !defined(LIL_BAKER)


#if defined(LIL_REFRACTION) && !defined(LIL_LITE)
/*!
 * @brief Calculate refraction.
 *
 * This function is optimized implemnetation of lilRefraction() defined in lil_common_frag.hlsl.
 * Use rsqrt().
 *
 * @param [in,out] fd  Fragment shader data.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Sampler stete (Optional argument).
 */
void lilRefractionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
{
    const float2 refractUV = fd.uvScn + (pow(1.0 - fd.nv, _RefractionFresnelPower) * _RefractionStrength) * mul((float3x3)LIL_MATRIX_V, fd.N).xy;
#ifdef LIL_REFRACTION_BLUR2
#    ifdef LIL_BRP
    const float blurOffset = fd.perceptualRoughness * rsqrt(fd.positionSS.w) * (0.03 / LIL_REFRACTION_SAMPNUM) * LIL_MATRIX_P._m11;
    float3 refractCol = 0.0;
    float sum = 0.0;
    for (int j = -16; j <= 16; j++) {
        refractCol += LIL_GET_GRAB_TEX(refractUV + float2(0, j * blurOffset), 0).rgb * LIL_REFRACTION_GAUSDIST(j);
        sum += LIL_REFRACTION_GAUSDIST(j);
    }
    refractCol /= sum;
    refractCol *= _RefractionColor.rgb;
#    else
    const float refractLod = min(sqrt(fd.perceptualRoughness * rsqrt(fd.positionSS.w) * 5.0), 10);
    float3 refractCol = LIL_GET_GRAB_TEX(refractUV, refractLod).rgb * _RefractionColor.rgb;
#    endif  // LIL_BRP
#else
    float3 refractCol = LIL_GET_BG_TEX(refractUV,0).rgb * _RefractionColor.rgb;
#endif  // LIL_REFRACTION_BLUR2
    if (_RefractionColorFromMain) {
        refractCol *= fd.albedo;
    }
    fd.col.rgb = lerp(refractCol, fd.col.rgb, fd.col.a);
}
#endif  // defined(LIL_REFRACTION) && !defined(LIL_LITE)


#if defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE) && !defined(LIL_FAKESHADOW) && !defined(LIL_BAKER)
/*!
 * @brief Calculate specular.
 *
 * This function is optimized implemnetation of lilCalcSpecular() defined in lil_common_frag.hlsl.
 * Vectorize calculation.
 *
 * @param [in,out] fd  Fragment shader data.
 * @param [in] L  Light direction.
 * @param [in] specular  Specular color.
 * @param [in] attenuation  Light attenuation.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Sampler stete (Optional argument).
 * @return Specular color.
 */
float3 lilCalcSpecularOverride(inout lilFragData fd, float3 L, float3 specular, float attenuation LIL_SAMP_IN_FUNC(samp))
{
    // Normal
#if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
    const float3 N = lerp(fd.origN, fd.N, _SpecularNormalStrength);
#else
    const float3 N = fd.N;
#endif  // defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)

    // Half direction
    const float3 H = normalize(fd.V + L);
    const float nh = saturate(dot(N, H));

    // Toon
#ifdef LIL_FEATURE_ANISOTROPY
    const bool isAnisotropy = _UseAnisotropy && _Anisotropy2Reflection;
    if (_SpecularToon & !isAnisotropy)
#else
    if (_SpecularToon)
#endif  // LIL_FEATURE_ANISOTROPY
    {
        return lilTooningScale(_AAStrength, pow(nh, 1.0 / fd.roughness), _SpecularBorder, _SpecularBlur);
    }

    // Dot
    const float nv = saturate(dot(N, fd.V));
    const float nl = saturate(dot(N, L));

    // GGX
    float ggx = 0.0;
    float lambdaV = 0.0;
    float lambdaL = 0.0;
#ifdef LIL_FEATURE_ANISOTROPY
    if (isAnisotropy) {
        const float2 roughnessTB = max(float2(
            fd.roughness + fd.roughness * fd.anisotropy,
            fd.roughness - fd.roughness * fd.anisotropy), 0.002);
        const float4 rds = roughnessTB.xyxy * float4(dot(fd.T, fd.V), dot(fd.B, fd.V), dot(fd.T, L), dot(fd.B, L));
        lambdaV = nl * length(float3(rds.x, rds.y, nv));
        lambdaL = nv * length(float3(rds.z, rds.w, nl));

        const float4 roughnessTB12 = float4(
            roughnessTB.x * _AnisotropyTangentWidth,
            roughnessTB.y * _AnisotropyBitangentWidth,
            roughnessTB.x * _Anisotropy2ndTangentWidth,
            roughnessTB.y * _Anisotropy2ndBitangentWidth);
#    ifdef LIL_FEATURE_AnisotropyShiftNoiseMask
        const float anisotropyShiftNoise = LIL_SAMPLE_2D_ST(_AnisotropyShiftNoiseMask, samp, fd.uvMain).r - 0.5;
#    else
        const float anisotropyShiftNoise = 0.5;
#    endif  // LIL_FEATURE_AnisotropyShiftNoiseMask
        const float anisotropyShift = anisotropyShiftNoise * _AnisotropyShiftNoiseScale + _AnisotropyShift;
        const float3 T1 = fd.T - N * anisotropyShift;
        const float3 B1 = fd.B - N * anisotropyShift;

        const float anisotropy2ndShift = anisotropyShiftNoise * _Anisotropy2ndShiftNoiseScale + _Anisotropy2ndShift;
        const float3 T2 = fd.T - N * anisotropy2ndShift;
        const float3 B2 = fd.B - N * anisotropy2ndShift;

        const float2 r12 = roughnessTB12.xz * roughnessTB12.yw;
        const float4 rsqrtTB12 = rsqrt(float4(dot(T1, T1), dot(B1, B1), dot(T2, T2), dot(B2, B2)));
        const float3 v1 = float3(float2(dot(T1 * rsqrtTB12.x, H), dot(B1 * rsqrtTB12.y, H)) * roughnessTB12.yx, nh * r12.x);
        const float3 v2 = float3(float2(dot(T2 * rsqrtTB12.z, H), dot(B2 * rsqrtTB12.y, H)) * roughnessTB12.wz, nh * r12.y);
        const float2 w12 = r12 / float2(dot(v1, v1), dot(v2, v2));
        const float2 w12Sq = w12 * w12;
        const float2 rw12 = r12 * w12Sq * float2(_AnisotropySpecularStrength, _Anisotropy2ndSpecularStrength);
        ggx = rw12.x + rw12.y;
    }
    else
#endif  // LIL_FEATURE_ANISOTROPY
    {
        const float roughness2 = max(fd.roughness, 0.002);
        const float nlnv = nl * nv;
        const float nlnvOneMinusRoughness = nlnv - nlnv * roughness2;
        lambdaV = nlnvOneMinusRoughness + nl * roughness2;
        lambdaL = nlnvOneMinusRoughness + nv * roughness2;

        const float r2 = roughness2 * roughness2;
        const float d = (nh * r2 - nh) * nh + 1.0;
        ggx = r2 / (d * d + 1e-7f);
    }

#if defined(SHADER_API_MOBILE) || defined(SHADER_API_SWITCH)
    const float sjggx = 0.5 / (lambdaV + lambdaL + 1e-4f);
#else
    const float sjggx = 0.5 / (lambdaV + lambdaL + 1e-5f);
#endif  // defined(SHADER_API_MOBILE) || defined(SHADER_API_SWITCH)

    float specularTerm = sjggx * ggx;
#ifdef LIL_COLORSPACE_GAMMA
    specularTerm = sqrt(max(1e-4h, specularTerm));
#endif  // LIL_COLORSPACE_GAMMA
    specularTerm *= nl * attenuation;

    // Output
#ifdef LIL_FEATURE_ANISOTROPY
    if (_SpecularToon) {
        return lilTooningScale(_AAStrength, specularTerm, 0.5);
    }
#endif  // LIL_FEATURE_ANISOTROPY

    const float lh = saturate(dot(L, H));
    return specularTerm * lilFresnelTerm(specular, lh);
}


/*!
 * @brief Calculate reflection.
 *
 * This function is optimized implemnetation of lilReflection() defined in lil_common_frag.hlsl.
 * Use lilCalcSpecularOverride().
 *
 * @param [in,out] fd  Fragment shader data.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Sampler stete (Optional argument).
 * @param [in] LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS  Position inputs.
 */
void lilReflectionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp) LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS)
{
#ifdef LIL_PASS_FORWARDADD
    if (_UseReflection && _ApplySpecular && _ApplySpecularFA)
#else
    if (_UseReflection)
#endif  // defined(LIL_PASS_FORWARDADD)
    {
        // Smoothness
#if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
        fd.smoothness = _Smoothness;
#    ifdef LIL_FEATURE_SmoothnessTex
        fd.smoothness *= LIL_SAMPLE_2D_ST(_SmoothnessTex, samp, fd.uvMain).r;
#    endif  // defined(LIL_FEATURE_SmoothnessTex)
        GSAAForSmoothness(fd.smoothness, fd.N, _GSAAStrength);
        fd.perceptualRoughness = fd.perceptualRoughness - fd.smoothness * fd.perceptualRoughness;
        fd.roughness = fd.perceptualRoughness * fd.perceptualRoughness;
#endif
        // Metallic
        float metallic = _Metallic;
#ifdef LIL_FEATURE_MetallicGlossMap
        metallic *= LIL_SAMPLE_2D_ST(_MetallicGlossMap, samp, fd.uvMain).r;
#endif  // defined(LIL_FEATURE_MetallicGlossMap)
        fd.col.rgb = fd.col.rgb - metallic * fd.col.rgb;
        const float3 specular = lerp(_Reflectance, fd.albedo, metallic);
        // Color
        float4 reflectionColor = _ReflectionColor;
#if defined(LIL_FEATURE_ReflectionColorTex)
        reflectionColor *= LIL_SAMPLE_2D_ST(_ReflectionColorTex, samp, fd.uvMain);
#endif  // defined(LIL_FEATURE_ReflectionColorTex)
#if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
        if (_ReflectionApplyTransparency) {
            reflectionColor.a *= fd.col.a;
        }
#endif  // LIL_RENDER == 2 && !defined(LIL_REFRACTION)

        // Specular
#ifndef LIL_PASS_FORWARDADD
        if (_ApplySpecular)
#endif  // !LIL_PASS_FORWARDADD
        {
#if 1
            const float3 lightDirectionSpc = fd.L;
            const float3 lightColorSpc = fd.lightColor;
#else
            const float3 lightDirectionSpc = lilGetLightDirection(fd.positionWS);
            const float3 lightColorSpc = LIL_MAINLIGHT_COLOR;
#endif
#if defined(LIL_PASS_FORWARDADD)
            const float3 reflectCol = lilCalcSpecularOverride(fd, lightDirectionSpc, specular, fd.shadowmix * fd.attenuation LIL_SAMP_IN(samp));
#elif defined(SHADOWS_SCREEN)
            const float3 reflectCol = lilCalcSpecularOverride(fd, lightDirectionSpc, specular, fd.shadowmix LIL_SAMP_IN(samp));
#else
            const float3 reflectCol = lilCalcSpecularOverride(fd, lightDirectionSpc, specular, 1.0 LIL_SAMP_IN(samp));
#endif  // defined(LIL_PASS_FORWARDADD)
            fd.col.rgb = lilBlendColor(fd.col.rgb, reflectionColor.rgb * lightColorSpc, reflectCol * reflectionColor.a, _ReflectionBlendMode);
        }
        // Reflection
#ifndef LIL_PASS_FORWARDADD
        if (_ApplyReflection) {
#    if defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
            const float3 N = lerp(fd.origN, fd.reflectionN, _ReflectionNormalStrength);
#    else
            const float3 N = fd.reflectionN;
#    endif  // defined(LIL_FEATURE_NORMAL_1ST) || defined(LIL_FEATURE_NORMAL_2ND)
            const float3 envReflectionColor = LIL_GET_ENVIRONMENT_REFLECTION(fd.V, N, fd.perceptualRoughness, fd.positionWS);

            const float oneMinusReflectivity = LIL_DIELECTRIC_SPECULAR.a - metallic * LIL_DIELECTRIC_SPECULAR.a;
            const float grazingTerm = saturate(fd.smoothness + (1.0 - oneMinusReflectivity));
#    ifdef LIL_COLORSPACE_GAMMA
            const float surfaceReduction = 1.0 - 0.28 * fd.roughness * fd.perceptualRoughness;
#    else
            const float surfaceReduction = 1.0 / (fd.roughness * fd.roughness + 1.0);
#    endif  // LIL_COLORSPACE_GAMMA

#    ifdef LIL_REFRACTION
            fd.col.rgb = lerp(envReflectionColor, fd.col.rgb, fd.col.a + (1.0 - fd.col.a) * pow(fd.nvabs, abs(_RefractionStrength) * 0.5 + 0.25));
            const float3 reflectCol = fd.col.a * surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, fd.nv);
            fd.col.a = 1.0;
#    else
            const float3 reflectCol = surfaceReduction * envReflectionColor * lilFresnelLerp(specular, grazingTerm, fd.nv);
#    endif  // LIL_REFRACTION
            fd.col.rgb = lilBlendColor(fd.col.rgb, reflectionColor.rgb, reflectCol * reflectionColor.a, _ReflectionBlendMode);
        }
#endif  // !LIL_PASS_FORWARDADD
    }
}
#endif  // defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE) && !defined(LIL_FAKESHADOW) && !defined(LIL_BAKER)


#endif  // LIL_OVERRIDE_INCLUDED
