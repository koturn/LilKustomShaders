#ifndef LIL_OPT_COMMON_FUNCTIONS_INCLUDED
#define LIL_OPT_COMMON_FUNCTIONS_INCLUDED


float lilAtanOpt(float x);
float lilAtanOpt(float x, float y);

float lilIsIn0to1Opt(float f);
float lilIsIn0to1Opt(float f, float nv);
float lilIsIn0to1Opt(float2 f);
float lilIsIn0to1Opt(float2 f, float nv);
float3 lilDecodeHDROpt(float4 data, float4 hdr);
float3 lilCustomReflectionOpt(TEXTURECUBE(tex), float4 hdr, float3 viewDirection, float3 normalDirection, float perceptualRoughness);
float3 lilToneCorrectionOpt(float3 c, float4 hsvg);
float2 lilGetPanoramaUVOpt(float3 viewDirection);
void lilPOMOpt(inout float2 uvMain, inout float2 uv, lilBool useParallax, float4 uv_st, float3 parallaxViewDirection, TEXTURE2D(parallaxMap), float parallaxScale, float parallaxOffsetParam);
void lilCalcDissolveOpt(
    inout float alpha,
    inout float dissolveAlpha,
    float2 uv,
    float3 positionOS,
    float4 dissolveParams,
    float4 dissolvePos,
    TEXTURE2D(dissolveMask),
    float4 dissolveMask_ST,
    bool dissolveMaskEnabled
    LIL_SAMP_IN_FUNC(samp));
void lilCalcDissolveWithNoiseOpt(
    inout float alpha,
    inout float dissolveAlpha,
    float2 uv,
    float3 positionOS,
    float4 dissolveParams,
    float4 dissolvePos,
    TEXTURE2D(dissolveMask),
    float4 dissolveMask_ST,
    bool dissolveMaskEnabled,
    TEXTURE2D(dissolveNoiseMask),
    float4 dissolveNoiseMask_ST,
    float4 dissolveNoiseMask_ScrollRotate,
    float dissolveNoiseStrength
    LIL_SAMP_IN_FUNC(samp));
float3 lilGetLightDirectionForSH9Opt();
float3 lilGetLightDirectionOpt(float4 lightDirectionOverride);
float3 lilGetFixedLightDirectionOpt(float4 lightDirectionOverride, bool doNormalise);
float4 lilVoronoiOpt(float2 pos, out float2 nearoffset, float scaleRandomize);
float3 lilCalcGlitterOpt(float2 uv, float3 normalDirection, float3 viewDirection, float3 cameraDirection, float3 lightDirection, float4 glitterParams1, float4 glitterParams2, float glitterPostContrast, float glitterSensitivity, float glitterScaleRandomize, uint glitterAngleRandomize, bool glitterApplyShape, TEXTURE2D(glitterShapeTex), float4 glitterShapeTex_ST, float4 glitterAtras);

#if !(defined(SHADER_API_D3D9) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS))
bool IsScreenTexOpt(TEXTURE2D(tex));
bool IsScreenTexOpt(TEXTURE2D_ARRAY(tex));
#endif
#ifdef LIL_BRP
void lilGetAdditionalLightsOpt(float3 positionWS, float4 positionCS, float strength, inout float3 lightColor, inout float3 lightDirection);;
float3 lilGetAdditionalLightsOpt(float3 positionWS, float4 positionCS, float strength);
float3 lilGetEnvReflectionOpt(float3 viewDirection, float3 normalDirection, float perceptualRoughness, float3 positionWS);
#endif  // LIL_BRP

half3 DecodeHDROpt(half4 data, half4 hdr);
float3 BoxProjectedCubemapDirectionOpt(float3 worldRefDir, float3 worldPos, float4 probePos, float4 boxMin, float4 boxMax);
half3 Unity_GlossyEnvironmentOpt(UNITY_ARGS_TEXCUBE(tex), half4 hdr, Unity_GlossyEnvironmentData glossIn);
half3 UnityGI_IndirectSpecularOpt(UnityGIInput data, half occlusion, Unity_GlossyEnvironmentData glossIn);


#define lilAtan lilAtanOpt

#define lilIsIn0to1 lilIsIn0to1Opt
#define lilDecodeHDR lilDecodeHDROpt
#define lilCustomReflection lilCustomReflectionOpt
#define lilToneCorrection lilToneCorrectionOpt
#define lilGetPanoramaUV lilGetPanoramaUVOpt
#define lilPOM lilPOMOpt
#define lilCalcDissolve lilCalcDissolveOpt
#define lilCalcDissolveWithNoise lilCalcDissolveWithNoiseOpt
#define lilGetSubTex lilGetSubTexOpt
#define lilGetSubTexWithoutAnimation lilGetSubTexWithoutAnimationOpt
#define lilGetLightDirectionForSH9 lilGetLightDirectionForSH9Opt
#define lilGetLightDirection lilGetLightDirectionOpt
#define lilGetFixedLightDirection lilGetFixedLightDirectionOpt
#define lilVoronoi lilVoronoiOpt
#define lilCalcGlitter lilCalcGlitterOpt

#if defined(SHADER_API_D3D9) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS)
// Replace (uv / LIL_SCREENPARAMS.xy) to (uv * LIL_SCREENPARAMS.zw - uv).
#    undef LIL_SAMPLE_2D_CS
#    undef LIL_SAMPLE_2D_ARRAY_CS
#    define LIL_SAMPLE_2D_CS(tex, uv)  tex2D(tex, uv * LIL_SCREENPARAMS.zw - uv)
#    define LIL_SAMPLE_2D_ARRAY_CS(tex, uv, index)  tex2DArray(tex, float3(uv * LIL_SCREENPARAMS.zw - uv, index))
#else
#    define IsScreenTex IsScreenTexOpt
#    define lilGetWidthAndHeight lilGetWidthAndHeightOpt
#endif
#define lilTransformCStoSSFrag lilTransformCStoSSFragOpt
#define lilCStoGrabUV lilCStoGrabUVOpt
#define lilLinearEyeDepth lilLinearEyeDepthOpt
#ifdef LIL_BRP
#    define lilGetAdditionalLights lilGetAdditionalLightsOpt
#    define lilGetEnvReflection lilGetEnvReflectionOpt
#    if !(defined(SHADOWS_DEPTH) && defined(SPOT) || defined(SHADOWS_SCREEN) && defined(UNITY_NO_SCREENSPACE_SHADOWS)) \
        && !defined(SHADOWS_CUBE) \
        && defined(SHADOWS_SCREEN)
// Replace (vi.positionCS.xy / LIL_SCREENPARAMS.xy) to (vi.positionCS.xy * LIL_SCREENPARAMS.zw - uv).
#        undef LIL_TRANSFER_SHADOW
#        define LIL_TRANSFER_SHADOW(vi, uv, o)  o._ShadowCoord = float4(vi.positionCS.xy * LIL_SCREENPARAMS.zw - vi.positionCS.xy, 1.0, 1.0);
#    endif
#endif  // LIL_BRP

#if defined(LIL_PASS_FORWARDADD)
#elif defined(LIL_HDRP) && defined(LIL_USE_LIGHTMAP)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SHADOWMASK)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
// Replace saturate((lightmapS.r + lightmapS.g + lightmapS.b) / 3.0) to saturate(dot(lightmapS, (1.0 / 3.0).xxx)).
#    undef LIL_GET_MAINLIGHT
#    define LIL_GET_MAINLIGHT(input,lc,ld,atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1, fd.uv2); \
        lc = max(lc, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld); \
        float3 lightmapShadowThreshold = LIL_MAINLIGHT_COLOR * 0.5; \
        float3 lightmapS = (lightmapColor - lightmapShadowThreshold) / (LIL_MAINLIGHT_COLOR - lightmapShadowThreshold); \
        float lightmapAttenuation = saturate(dot(lightmapS, (1.0 / 3.0).xxx)); \
        atten = min(atten, lightmapAttenuation)
#elif defined(LIL_USE_LIGHTMAP) && defined(LIL_LIGHTMODE_SUBTRACTIVE)
// Replace saturate((lightmapS.r + lightmapS.g + lightmapS.b) / 3.0) to saturate(dot(lightmapS, (1.0 / 3.0).xxx)).
#    undef LIL_GET_MAINLIGHT
#    define LIL_GET_MAINLIGHT(input, lc, ld, atten) \
        lc = input.lightColor; \
        float3 lightmapColor = lilGetLightMapColor(fd.uv1, fd.uv2); \
        lc = max(lc, lightmapColor); \
        LIL_CORRECT_LIGHTCOLOR_PS(lc); \
        LIL_CORRECT_LIGHTDIRECTION_PS(ld); \
        float3 lightmapS = (lightmapColor - lilShadeSH9(input.normalWS)) / LIL_MAINLIGHT_COLOR; \
        float lightmapAttenuation = saturate(dot(lightmapS, (1.0 / 3.0).xxx)); \
        atten = min(atten, lightmapAttenuation)
#endif

#if defined(LIL_PASS_FORWARDADD)
#elif defined(LIL_HDRP)
// Not optimize, just for more precise.
// Replace 0.333333 (0.33333299; 0x3eaaaa9f) to (1.0 / 3.0) (0.33333334; 0x3eaaaaab)
// and 0.666666 (0.66666597; 0x3f2aaa9f) to (2.0 / 3.0) (0.66666669; 0x3f2aaaab).
#    undef LIL_CALC_MAINLIGHT
#    define LIL_CALC_MAINLIGHT(i, o) \
        lilLightData o; \
        lilGetLightDirectionAndColor(o.lightDirection, o.lightColor, posInput); \
        o.lightColor *= _lilDirectionalLightStrength; \
        float3 lightDirectionCopy = o.lightDirection; \
        o.lightDirection = o.lightDirection * Luminance(o.lightColor) + unity_SHAr.xyz * (1.0 / 3.0) + unity_SHAg.xyz * (1.0 / 3.0) + unity_SHAb.xyz * (1.0 / 3.0); \
        float3 lightDirectionSH = dot(o.lightDirection,o.lightDirection) < 0.000001 ? 0 : normalize(o.lightDirection); \
        o.lightDirection += lilGetCustomLightDirection(_LightDirectionOverride); \
        o.lightColor += lilShadeSH9(float4(lightDirectionSH * (2.0 / 3.0), 1.0)); \
        o.indLightColor = lilShadeSH9(float4(-lightDirectionSH * (2.0 / 3.0), 1.0)); \
        o.indLightColor = saturate(o.indLightColor / Luminance(o.lightColor)); \
        o.lightColor = min(o.lightColor, _BeforeExposureLimit); \
        o.lightColor *= GetCurrentExposureMultiplier(); \
        LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i, o); \
        LIL_CORRECT_LIGHTCOLOR_VS(o.lightColor)
#elif defined(LIL_BRP)
#else
// Not optimize, just for more precise.
// Replace 0.333333 (0.33333299; 0x3eaaaa9f) to (1.0 / 3.0) (0.33333334; 0x3eaaaaab)
#    undef LIL_CALC_MAINLIGHT
#    define LIL_CALC_MAINLIGHT(i, o) \
        lilLightData o; \
        o.lightDirection = normalize(LIL_MAINLIGHT_DIRECTION * lilLuminance(LIL_MAINLIGHT_COLOR) + unity_SHAr.xyz * (1.0 / 3.0) + unity_SHAg.xyz * (1.0 / 3.0) + unity_SHAb.xyz * (1.0 / 3.0)); \
        LIL_CALC_TWOLIGHT(i, o); \
        o.lightDirection = lilGetFixedLightDirection(_LightDirectionOverride, false); \
        LIL_APPLY_ADDITIONALLIGHT_TO_MAIN(i, o); \
        LIL_CORRECT_LIGHTCOLOR_VS(o.lightColor)
#endif


#define DecodeHDR DecodeHDROpt
#define BoxProjectedCubemapDirection BoxProjectedCubemapDirectionOpt
#define Unity_GlossyEnvironment Unity_GlossyEnvironmentOpt
#define UnityGI_IndirectSpecular UnityGI_IndirectSpecularOpt


/*!
 * @brief Fast atan().
 *
 * This function is just for overloading.
 *
 * @param [in] x  The first argument of atan().
 * @return Approximate value of atan().
 * @see https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/
 */
float lilAtanOpt(float x)
{
    float t0 = lilAtanPos(abs(x));
    return (x < 0.0) ? -t0 : t0;
}


/*!
 * @brief Fast atan2().
 *
 * This function is optimized implemnetation of lilAtan() defined in lil_common_functions_thirdparty.hlsl.
 * Reduce one "div" instruction and optimize last "cmov".
 *
 * @param [in] x  The first argument of atan2().
 * @param [in] y  The second argument of atan2().
 * @return Approximate value of atan().
 * @see https://seblagarde.wordpress.com/2014/12/01/inverse-trigonometric-functions-gpu-optimization-for-amd-gcn-architecture/
 */
float lilAtanOpt(float x, float y)
{
    const float2 p = float2(x, y);
    const float2 d = p.xy / p.yx;

    //
    // Tune for positive input [0, infinity] and provide output [0, PI/2]
    //
    const float2 absD = abs(d);
    const float t0 = absD.x < 1.0 ? absD.x : absD.y;
#if 1
    const float poly = (-0.269408 * t0 + 1.05863) * t0;
#else
    const float t1 = t0 * t0;
    float poly = 0.0872929;
    poly = -0.301895 + poly * t1;
    poly = 1.0 + poly * t1;
    poly *= t0;
#endif
    const float u0 = absD < 1.0 ? poly : (LIL_HALF_PI - poly);

    return (d >= 0.0 ? u0 : -u0) + (y >= 0.0 ? 0.0 : x >= 0.0 ? LIL_PI : -LIL_PI);
}


#if LIL_ANTIALIAS_MODE == 0
/*!
 * @brief Determine whether specified scalar is in [0, 1] or not.
 *
 * This function is just for overloading.
 *
 * @param [in] f  Target scalar.
 * @return 1.0 if specified value is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float f)
{
    return saturate(f) == f;
}


/*!
 * @brief Determine whether specified scalar is in [0, 1] or not.
 *
 * This function is just for overloading.
 *
 * @param [in] f  Target scalar.
 * @param [in] nv  Upper value of clamp() for result of fwidth(). (Unused)
 * @return 1.0 if specified value is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float f, float nv)
{
    return lilIsIn0to1Opt(f);
}


/*!
 * @brief Determine whether each specified vector component is in [0, 1] or not.
 *
 * This function is vectorised implemnetation of lilIsIn0to1() defined in lil_common_functions.hlsl.
 * !!NO EFFECT ON OPTIMIZATION...?!!
 *
 * @param [in] f  Target vector.
 * @return 1.0 if the component is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float2 f)
{
    const float2 results = saturate(f) == f;
    return results.x * results.y;
}


/*!
 * @brief Determine whether each specified vector component is in [0, 1] or not.
 *
 * This function is vectorised implemnetation of lilIsIn0to1() defined in lil_common_functions.hlsl.
 * !!NO EFFECT ON OPTIMIZATION...?!!
 *
 * @param [in] f  Target vector.
 * @param [in] nv  Upper value of clamp() for result of fwidth(). (Unused)
 * @return 1.0 if the component is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float2 f, float nv)
{
    return lilIsIn0to1Opt(f);
}
#else
/*!
 * @brief Determine whether specified scalar is in [0, 1] or not considering anti-aliasing.
 *
 * This function is just for overloading.
 *
 * @param [in] f  Target scalar.
 * @return 1.0 if specified value is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float f)
{
    return lilIsIn0to1Opt(f, 1.0);
}


/*!
 * @brief Determine whether specified scalar is in [0, 1] or not considering anti-aliasing.
 *
 * This function is just for overloading.
 *
 * @param [in] f  Target scalar.
 * @param [in] nv  Upper value of clamp() for result of fwidth().
 * @return 1.0 if specified value is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float f, float nv)
{
    const float value = 0.5 - abs(f - 0.5);
    return saturate(value / clamp(fwidth(value), 0.0001, nv));
}


/*!
 * @brief Determine whether each specified vector component is in [0, 1] or not considering anti-aliasing.
 *
 * This function is vectorised implemnetation of lilIsIn0to1() defined in lil_common_functions.hlsl.
 * !!NO EFFECT ON OPTIMIZATION...?!!
 *
 * @param [in] f  Target vector.
 * @return 1.0 if the component is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float2 f)
{
    return lilIsIn0to1Opt(f, 1.0);
}


/*!
 * @brief Determine whether each specified vector component is in [0, 1] or not considering anti-aliasing.
 *
 * This function is vectorised implemnetation of lilIsIn0to1() defined in lil_common_functions.hlsl.
 * !!NO EFFECT ON OPTIMIZATION...?!!
 *
 * @param [in] f  Target vector.
 * @param [in] nv  Upper value of clamp() for result of fwidth().
 * @return 1.0 if the component is in [0, 1], otherwise 0.0.
 */
float lilIsIn0to1Opt(float2 f, float nv)
{
    const float2 value = 0.5 - abs(f - 0.5);
    const float2 results = saturate(value / clamp(fwidth(value), 0.0001, nv));
    return results.x * results.y;
}
#endif  // LIL_ANTIALIAS_MODE == 0


/*!
 * @brief Decodes HDR textures; handles dLDR, RGBM formats.
 *
 * This function is optimized implemnetation of lilDecodeHDR() defined in lil_common_functions.hlsl.
 * "MAD"nize in code pass of UNITY_COLORSPACE_GAMMA.
 *
 * @param [in] data  HDR color data.
 * @param [in] hdr  Decode instruction.
 * @return Decoded color data.
 */
float3 lilDecodeHDROpt(float4 data, float4 hdr)
{
#if defined(LIL_COLORSPACE_GAMMA)
    return (hdr.x * (hdr.w * data.a - hdr.w) + hdr.x) * data.rgb;
#elif defined(UNITY_USE_NATIVE_HDR)
    return hdr.x * data.rgb;
#else
    const float alpha = hdr.w * (data.a - 1.0) + 1.0;
    return (hdr.x * pow(abs(alpha), hdr.y)) * data.rgb;
#endif  // defined(LIL_COLORSPACE_GAMMA)
}


/*!
 * @brief Pick color data from cube map texture.
 *
 * This function is optimized implemnetation of lilCustomReflection() defined in lil_common_functions.hlsl.
 * Use lilDecodeHDROpt().
 *
 * @param [in] TEXTURECUBE(tex)  Cube map texture.
 * @param [in] hdr  Decode instruction.
 * @param [in] viewDirection  View direction.
 * @param [in] normalDirection  Normal direction.
 * @param [in] perceptualRoughness  Preceptual roughness.
 * @return Decoded color data.
 * @see lilDecodeHDROpt
 */
float3 lilCustomReflectionOpt(TEXTURECUBE(tex), float4 hdr, float3 viewDirection, float3 normalDirection, float perceptualRoughness)
{
    const float mip = perceptualRoughness * (10.2 - 4.2 * perceptualRoughness);
    const float3 refl = reflect(-viewDirection, normalDirection);
    return lilDecodeHDROpt(LIL_SAMPLE_CUBE_LOD(tex, lil_sampler_linear_repeat, refl, mip), hdr);
}


/*!
 * @brief Correct color tone.
 *
 * This function is optimized implemnetation of lilToneCorrection() defined in lil_common_functions.hlsl.
 * Reduce move instruction and reduce div instruction using vector operation.
 *
 * @param [in] c  RGB color.
 * @param [in] hsvg  HSV and Gamma correction values.
 * @return Corrected RGB color.
 */
float3 lilToneCorrectionOpt(float3 c, float4 hsvg)
{
    static const float4 k1 = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    static const float4 k2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    static const float e = 1.0e-10;

    //
    // gamma
    //
    c = pow(abs(c), hsvg.w);

    //
    // rgb -> hsv
    //
    const bool b1 = c.g < c.b;
    float4 p1 = float4(b1 ? c.bg : c.gb, b1 ? k1.wz : k1.xy);

    const bool b2 = c.r < p1.x;
    p1.xyz = b2 ? p1.xyw : p1.yzx;
    const float4 q = b2 ? float4(p1.xyz, c.r) : float4(c.r, p1.xyz);

    const float d = q.x - min(q.w, q.y);
    const float2 hs = float2(q.w - q.y, d) / float2(6.0 * d + e, q.x + e);

    float3 hsv = float3(abs(q.z + hs.x), hs.y, q.x);

    //
    // shift
    //
    hsv = float3(hsv.x + hsvg.x, saturate(hsv.y * hsvg.y), saturate(hsv.z * hsvg.z));

    //
    // hsv -> rgb
    //
    const float3 p2 = abs(frac(hsv.xxx + k2.xyz) * 6.0 - k2.www);
    return hsv.z * lerp(k2.xxx, saturate(p2 - k2.xxx), hsv.y);
}


/*!
 * @brief Get panorama UV coordinate.
 *
 * This function is optimized implemnetation of lilGetPanoramaUV() defined in lil_common_functions.hlsl.
 * Optimize loop breaking.
 *
 * @param [in] viewDirection  View direction.
 * @return Panorama UV.
 */
float2 lilGetPanoramaUVOpt(float3 viewDirection)
{
    return float2(lilAtanOpt(viewDirection.x, viewDirection.z), lilAcos(viewDirection.y)) * LIL_INV_PI;
}


/*!
 * @brief Calculate Parallax Occulusion Mapping.
 *
 * This function is optimized implemnetation of lilPOM() defined in lil_common_functions.hlsl.
 * Optimize loop breaking.
 *
 * @param [in,out] uvMain  Main UV coordinate.
 * @param [in,out] uv  UV coordinate.
 * @param [in] useParallax  Switch flag whether use parallax or not.
 * @param [in] uv_st  Tiling and Offset of uv.
 * @param [in] parallaxViewDirection  Parallax view direction.
 * @param [in] TEXTURE2D(parallaxMap)  Parallax map.
 * @param [in] parallaxScale  Parallax scale.
 * @param [in] parallaxOffsetParam  Parallax offset parameter.
 */
void lilPOMOpt(inout float2 uvMain, inout float2 uv, lilBool useParallax, float4 uv_st, float3 parallaxViewDirection, TEXTURE2D(parallaxMap), float parallaxScale, float parallaxOffsetParam)
{
    static const int kPomDetail = 200;

    if (useParallax) {
        float3 rayPos = float3(uvMain, 1.0) + (parallaxScale - parallaxOffsetParam * parallaxScale) * parallaxViewDirection;
        float3 rayStep = -parallaxViewDirection;
        rayStep.xy *= uv_st.xy;
        rayStep = rayStep / kPomDetail;
        rayStep.z /= parallaxScale;

        float height;
        float height2;
        for (int i = 0; i < kPomDetail * 2 * parallaxScale; i = (height >= rayPos.z ? 0x7fffffff : i + 1)) {
            height2 = height;
            rayPos += rayStep;
            height = LIL_SAMPLE_2D_LOD(parallaxMap, lil_sampler_linear_repeat, rayPos.xy, 0).r;
        }

        const float2 prevObjPoint = rayPos.xy - rayStep.xy;
        const float nextHeight = height - rayPos.z;
        const float prevHeight = height2 - rayPos.z + rayStep.z;

        const float weight = nextHeight / (nextHeight - prevHeight);
        rayPos.xy = lerp(rayPos.xy, prevObjPoint, weight);

        uv += rayPos.xy - uvMain;
        uvMain = rayPos.xy;
    }
}


/*!
 * @brief Calculate dissolve.
 *
 * This function is optimized implemnetation of lilCalcDissolve() defined in lil_common_functions.hlsl.
 * Avoid to use div with same divisor. Calculate reciprocal of divisor and mul with it.
 *
 * @param [in,out] alpha  Alpha value.
 * @param [in,out] dissolveAlpha  Dissolve alpha value.
 * @param [in] uv  UV coordinate.
 * @param [in] positionOS  Object space position.
 * @param [in] dissolveParams  Dissolve parameters.
 * @param [in] dissolvePos  Dissolve position.
 * @param [in] TEXTURE2D(dissolveMask)  Dissolve mask texture.
 * @param [in] dissolveMask_ST  Tilling and Offset of dissolveMask
 * @param [in] dissolveMaskEnabled  A switch flag whether dissolve mask is enabled or not.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Texture sampler.
 */
void lilCalcDissolveOpt(
    inout float alpha,
    inout float dissolveAlpha,
    float2 uv,
    float3 positionOS,
    float4 dissolveParams,
    float4 dissolvePos,
    TEXTURE2D(dissolveMask),
    float4 dissolveMask_ST,
    bool dissolveMaskEnabled
    LIL_SAMP_IN_FUNC(samp))
{
    if (dissolveParams.r) {
        float dissolveMaskVal = 1.0;
        if (dissolveParams.r == 1.0 && dissolveMaskEnabled) {
            dissolveMaskVal = LIL_SAMPLE_2D(dissolveMask, samp, lilCalcUV(uv, dissolveMask_ST)).r;
        }

        const float invA = rcp(dissolveParams.a);
        if (dissolveParams.r == 1.0) {
            dissolveAlpha = 1.0 - saturate(abs(dissolveMaskVal - dissolveParams.b) * invA);
            dissolveMaskVal = dissolveMaskVal > dissolveParams.b ? 1.0 : 0.0;
        }
        if (dissolveParams.r == 2.0) {
            dissolveAlpha = dissolveParams.g == 1.0 ? lilRotateUV(uv, dissolvePos.w).x : distance(uv, dissolvePos.xy);
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) * invA);
        }
        if (dissolveParams.r == 3.0) {
            dissolveAlpha = dissolveParams.g == 1.0 ? dot(positionOS, normalize(dissolvePos.xyz)).x : distance(positionOS, dissolvePos.xyz);
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) * invA);
        }
        alpha *= dissolveMaskVal;
    }
}


/*!
 * @brief Calculate dissolve with noise.
 *
 * This function is optimized implemnetation of lilCalcDissolve() defined in lil_common_functions.hlsl.
 * Avoid to use div with same divisor. Calculate reciprocal of divisor and mul with it.
 *
 * @param [in,out] alpha  Alpha value.
 * @param [in,out] dissolveAlpha  Dissolve alpha value.
 * @param [in] uv  UV coordinate.
 * @param [in] positionOS  Object space position.
 * @param [in] dissolveParams  Dissolve parameters.
 * @param [in] dissolvePos  Dissolve position.
 * @param [in] TEXTURE2D(dissolveMask)  Dissolve mask texture.
 * @param [in] dissolveMask_ST  Tilling and Offset of dissolveMask
 * @param [in] dissolveMaskEnabled  A switch flag whether dissolve mask is enabled or not.
 * @param [in] TEXTURE2D(dissolveNoiseMask)  Noise mask texture.
 * @param [in] float4 dissolveNoiseMask_ST  Tilling and Offset of noise mask.
 * @param [in] float4 dissolveNoiseMask_ScrollRotate  Scroll and Rotate parameters of dissolveNoiseMask.
 * @param [in] float dissolveNoiseStrength  Noise strength.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Texture sampler.
 */
void lilCalcDissolveWithNoiseOpt(
    inout float alpha,
    inout float dissolveAlpha,
    float2 uv,
    float3 positionOS,
    float4 dissolveParams,
    float4 dissolvePos,
    TEXTURE2D(dissolveMask),
    float4 dissolveMask_ST,
    bool dissolveMaskEnabled,
    TEXTURE2D(dissolveNoiseMask),
    float4 dissolveNoiseMask_ST,
    float4 dissolveNoiseMask_ScrollRotate,
    float dissolveNoiseStrength
    LIL_SAMP_IN_FUNC(samp))
{
    if (dissolveParams.r) {
        float dissolveMaskVal = 1.0;
        float dissolveNoise = 0.0;
        if (dissolveParams.r == 1.0 && dissolveMaskEnabled) {
            dissolveMaskVal = LIL_SAMPLE_2D(dissolveMask, samp, lilCalcUV(uv, dissolveMask_ST)).r;
        }
        dissolveNoise = LIL_SAMPLE_2D(dissolveNoiseMask, samp, lilCalcUV(uv, dissolveNoiseMask_ST, dissolveNoiseMask_ScrollRotate.xy)).r - 0.5;
        dissolveNoise *= dissolveNoiseStrength;

        const float invA = rcp(dissolveParams.a);
        if (dissolveParams.r == 1.0) {
            dissolveAlpha = 1.0 - saturate(abs(dissolveMaskVal + dissolveNoise - dissolveParams.b) * invA);
            dissolveMaskVal = dissolveMaskVal + dissolveNoise > dissolveParams.b ? 1.0 : 0.0;
        }
        if (dissolveParams.r == 2.0) {
            dissolveAlpha = dissolveParams.g == 1.0 ? dot(uv, normalize(dissolvePos.xy)) + dissolveNoise : distance(uv, dissolvePos.xy) + dissolveNoise;
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) * invA);
        }
        if (dissolveParams.r == 3.0) {
            dissolveAlpha = dissolveParams.g == 1.0 ? dot(positionOS, normalize(dissolvePos.xyz)) + dissolveNoise : distance(positionOS, dissolvePos.xyz) + dissolveNoise;
            dissolveMaskVal *= dissolveAlpha > dissolveParams.b ? 1.0 : 0.0;
            dissolveAlpha = 1.0 - saturate(abs(dissolveAlpha - dissolveParams.b) * invA);
        }
        alpha *= dissolveMaskVal;
    }
}





/*!
 * @brief Sample sub texture.
 *
 * Use lilIsIn0to1Opt().
 * !!NO EFFECT ON OPTIMIZATION...?!!
 *
 * @param [in] TEXTURE2D(tex)
 * @param [in] TEXTURE2D(tex)  2D texture.
 * @param [in] uv_ST  Tilling and Offset parameters.
 * @param [in] uv_SR  Scroll and Rotate parameters.
 * @param [in] angle  Rotate angle.
 * @param [in] uv  UV coordinate.
 * @param [in] nv  Value for clamp() for result of fwidth().
 * @param [in] isDecal  Is decal texture or not.
 * @param [in] isLeftOnly  Is left only or not.
 * @param [in] isRightOnly  Is right only or not.
 * @param [in] shouldCopy  Should copy or not.
 * @param [in] shouldFlipMirror  Should flip in mirror ot not.
 * @param [in] shouldFlipCopy  Should flip copy ot not.
 * @param [in] isMSDF  Is MSDF or not.
 * @param [in] isRightHand  Is right-handed system or not (Is in mirror or not).
 * @param [in] decalAnimation  Decal animation parameters.
 * @param [in] decalSubParam  Decal sub parameters.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Texture sampler.
 * @return Sampled color.
 * @see lilIsIn0to1Opt
 */
float4 lilGetSubTexOpt(
    TEXTURE2D(tex),
    float4 uv_ST,
    float4 uv_SR,
    float angle,
    float2 uv,
    float nv,
    bool isDecal,
    bool isLeftOnly,
    bool isRightOnly,
    bool shouldCopy,
    bool shouldFlipMirror,
    bool shouldFlipCopy,
    bool isMSDF,
    bool isRightHand,
    float4 decalAnimation,
    float4 decalSubParam
    LIL_SAMP_IN_FUNC(samp))
{
#ifdef LIL_FEATURE_DECAL
    const float4 uv_SR2 = float4(uv_SR.xy, angle, uv_SR.w);
    const float2 uv2 = lilCalcDecalUV(uv, uv_ST, uv_SR2, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
#    ifdef LIL_FEATURE_ANIMATE_DECAL
    const float2 uv2samp = lilCalcAtlasAnimation(uv2, decalAnimation, decalSubParam);
#    else
    const float2 uv2samp = uv2;
#    endif  // LIL_FEATURE_ANIMATE_DECAL
    float4 outCol = LIL_SAMPLE_2D(tex, samp, uv2samp);
    if (isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
    if (isDecal) outCol.a *= lilIsIn0to1Opt(uv2, saturate(nv - 0.05));
    return outCol;
#else
    const float4 uv_SR2 = float4(uv_SR.xy, angle, uv_SR.w);
    const float2 uv2 = lilCalcUV(uv, uv_ST, uv_SR2);
    float4 outCol = LIL_SAMPLE_2D(tex, samp,uv2);
    if (isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
    return outCol;
#endif  // LIL_FEATURE_DECAL
}


/*!
 * @brief Sample sub texture.
 *
 * Use lilIsIn0to1Opt().
 * !!NO EFFECT ON OPTIMIZATION...?!!
 *
 * @param [in] TEXTURE2D(tex)
 * @param [in] TEXTURE2D(tex)  2D texture.
 * @param [in] uv_ST  Tilling and Offset parameters.
 * @param [in] uv_SR  Scroll and Rotate parameters.
 * @param [in] angle  Rotate angle.
 * @param [in] uv  UV coordinate.
 * @param [in] nv  Value for clamp() for result of fwidth().
 * @param [in] isDecal  Is decal texture or not.
 * @param [in] isLeftOnly  Is left only or not.
 * @param [in] isRightOnly  Is right only or not.
 * @param [in] shouldCopy  Should copy or not.
 * @param [in] shouldFlipMirror  Should flip in mirror ot not.
 * @param [in] shouldFlipCopy  Should flip copy ot not.
 * @param [in] isMSDF  Is MSDF or not.
 * @param [in] isRightHand  Is right-handed system or not (Is in mirror or not).
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Texture sampler.
 * @return Sampled color.
 * @see lilIsIn0to1Opt
 */
float4 lilGetSubTexWithoutAnimationOpt(
    TEXTURE2D(tex),
    float4 uv_ST,
    float4 uv_SR,
    float angle,
    float2 uv,
    float nv,
    bool isDecal,
    bool isLeftOnly,
    bool isRightOnly,
    bool shouldCopy,
    bool shouldFlipMirror,
    bool shouldFlipCopy,
    bool isMSDF,
    bool isRightHand
    LIL_SAMP_IN_FUNC(samp))
{
#ifdef LIL_FEATURE_DECAL
    const float2 uv2 = lilCalcDecalUV(uv, uv_ST, angle, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, isRightHand);
    float4 outCol = LIL_SAMPLE_2D(tex, samp, uv2);
    if (isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
    if (isDecal) outCol.a *= lilIsIn0to1Opt(uv2, saturate(nv - 0.05));
    return outCol;
#else
    const float2 uv2 = lilCalcUV(uv, uv_ST, angle);
    float4 outCol = LIL_SAMPLE_2D(tex, samp, uv2);
    if (isMSDF) outCol = float4(1.0, 1.0, 1.0, lilMSDF(outCol.rgb));
    return outCol;
#endif  // LIL_FEATURE_DECAL
}


/*!
 * @brief Get light direction for SH9.
 *
 * This function is more precise implementation of lilGetLightDirectionForSH9() defined in lil_common_functions.hlsl.
 * Replace 0.333333 (0.33333299; 0x3eaaaa9f) to (1.0 / 3.0) (0.33333334; 0x3eaaaaab).
 *
 * @return Light direction for SH9.
 */
float3 lilGetLightDirectionForSH9Opt()
{
    static const float kOneThird = 1.0 / 3.0;
    const float3 mainDir = LIL_MAINLIGHT_DIRECTION * lilLuminance(LIL_MAINLIGHT_COLOR);
    const float3 sh9Dir = unity_SHAr.xyz * kOneThird + unity_SHAg.xyz * kOneThird + unity_SHAb.xyz * kOneThird;
    const float3 lightDirectionForSH9 = sh9Dir + mainDir;
    return dot(lightDirectionForSH9, lightDirectionForSH9) < 0.000001 ? 0 : normalize(lightDirectionForSH9);
}


/*!
 * @brief Get light direction.
 *
 * This function is more precise implementation of lilGetLightDirection() defined in lil_common_functions.hlsl.
 * Replace 0.333333 (0.33333299; 0x3eaaaa9f) to (1.0 / 3.0) (0.33333334; 0x3eaaaaab).
 *
 * @param [in] lightDirectionOverride  Correct term of light direction.
 * @return Light direction.
 */
float3 lilGetLightDirectionOpt(float4 lightDirectionOverride)
{
    static const float kOneThird = 1.0 / 3.0;
    const float3 mainDir = LIL_MAINLIGHT_DIRECTION * lilLuminance(LIL_MAINLIGHT_COLOR);
    const float3 sh9Dir = unity_SHAr.xyz * kOneThird + unity_SHAg.xyz * kOneThird + unity_SHAb.xyz * kOneThird;
    return normalize(mainDir + sh9Dir + lilGetCustomLightDirection(lightDirectionOverride));
}


/*!
 * @brief Get light direction.
 *
 * This function is just for overloading.
 *
 * @return Light direction.
 */
float3 lilGetLightDirection()
{
    return lilGetLightDirectionOpt(float4(0.001, 0.002, 0.001, 0.0));
}


/*!
 * @brief Get light direction.
 *
 * This function is just for overloading.
 *
 * @param [in] positionWS  World space position.
 * @return Light direction.
 */
float3 lilGetLightDirectionOpt(float3 positionWS)
{
#if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
    return normalize(LIL_MAINLIGHT_DIRECTION - positionWS);
#else
    return LIL_MAINLIGHT_DIRECTION;
#endif  // defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
}


/*!
 * @brief Get fixed light direction.
 *
 * This function is more precise implementation of lilGetFixedLightDirection() defined in lil_common_functions.hlsl.
 * Replace 0.333333 (0.33333299; 0x3eaaaa9f) to (1.0 / 3.0) (0.33333334; 0x3eaaaaab).
 *
 * @param [in] lightDirectionOverride  Correct term of light direction.
 * @param [in] doNormalize  A flag whether do normalize() or not.
 * @return Fixed light direction.
 */
float3 lilGetFixedLightDirectionOpt(float4 lightDirectionOverride, bool doNormalise)
{
    static const float kOneThird = 1.0 / 3.0;
    const float3 mainDir = LIL_MAINLIGHT_DIRECTION * lilLuminance(LIL_MAINLIGHT_COLOR);
    const float3 sh9Dir = unity_SHAr.xyz * kOneThird + unity_SHAg.xyz * kOneThird + unity_SHAb.xyz * kOneThird;
    const float3 L = float3(sh9Dir.x, abs(sh9Dir.y), sh9Dir.z) + mainDir + lilGetCustomLightDirection(lightDirectionOverride);
    return doNormalise ? normalize(L) : L;
}


/*!
 * @brief Get light direction.
 *
 * This function is just for overloading.
 *
 * @param [in] lightDirectionOverride  Correct term of light direction.
 * @return Fixed light direction (normalized).
 */
float3 lilGetFixedLightDirectionOpt(float4 lightDirectionOverride)
{
    return lilGetFixedLightDirectionOpt(lightDirectionOverride, true);
}


/*!
 * @brief Calculate volonoi division.
 *
 * This function is optimized implementation of lilVoronoi() defined in lil_common_functions.hlsl.
 * Vectorize calculation in the code pass, defined(SHADER_API_D3D9) || defined(SHADER_API_D3D11_9X).
 *
 * @param [in] pos  Position.
 * @param [out] nearoffset  Near offset.
 * @param [in] scaleRandomize  Scale for randomization.
 * @return Volonoi division result.
 */
float4 lilVoronoiOpt(float2 pos, out float2 nearoffset, float scaleRandomize)
{
#if defined(SHADER_API_D3D9) || defined(SHADER_API_D3D11_9X)
    static const float3 ms = float3(46203.4357, 21091.5327, 35771.1966);
    static const float2 ks = float2(12.9898, 78.233);
    const float2 q = trunc(pos);
    const float4 q2 = float4(q.x, q.y, q.x + 1.0, q.y + 1.0);
    const float4 rs = sin(float4(dot(q2.xy, ks), dot(q2.zy, ks), dot(q2.xw, ks), dot(q2.zw, ks)));
    const float3 noise0 = frac(rs.x * ms);
    const float3 noise1 = frac(rs.y * ms);
    const float3 noise2 = frac(rs.z * ms);
    const float3 noise3 = frac(rs.w * ms);
#else
    float3 noise0, noise1, noise2, noise3;
    lilHashRGB4(pos, noise0, noise1, noise2, noise3);
#endif  // defined(SHADER_API_D3D9) || defined(SHADER_API_D3D11_9X)

    // Get the nearest position
    const float4 fracpos = frac(pos).xyxy + float4(0.5, 0.5, -0.5, -0.5);
    float4 dist4 = float4(
        lilNsqDistance(fracpos.xy, noise0.xy),
        lilNsqDistance(fracpos.zy, noise1.xy),
        lilNsqDistance(fracpos.xw, noise2.xy),
        lilNsqDistance(fracpos.zw, noise3.xy));
    dist4 = lerp(dist4, dist4 / max(float4(noise0.z, noise1.z, noise2.z, noise3.z), 0.001), scaleRandomize);

    const float3 nearoffset0 = dist4.x < dist4.y ? float3(0, 0, dist4.x) : float3(1, 0, dist4.y);
    const float3 nearoffset1 = dist4.z < dist4.w ? float3(0, 1, dist4.z) : float3(1, 1, dist4.w);
    nearoffset = nearoffset0.z < nearoffset1.z ? nearoffset0.xy : nearoffset1.xy;

    const float4 near0 = dist4.x < dist4.y ? float4(noise0, dist4.x) : float4(noise1, dist4.y);
    const float4 near1 = dist4.z < dist4.w ? float4(noise2, dist4.z) : float4(noise3, dist4.w);
    return near0.w < near1.w ? near0 : near1;
}


/*!
 * @brief Calculate glitter.
 *
 * This function is optimized implemnetation of lilCalcGlitter() defined in lil_common_functions.hlsl.
 * Use rsqrt().
 *
 * @param [in] uv  UV coordinate.
 * @param [in] normalDirection  Normal direction.
 * @param [in] viewDirection  View direction.
 * @param [in] cameraDirection  Camera direction.
 * @param [in] lightDirection  Light direction.
 * @param [in] glitterParams1  Glitter parameter vector (x: Scale, y: Scale, z: Size, w: Contrast)
 * @param [in] glitterParams2  Glitter parameter vector (x: Speed, y: Angle, z: Light Direction, w:)
 * @param [in] glitterPostContrast  Post contrast.
 * @param [in] glitterSensitivity  Sensitivity
 * @param [in] glitterScaleRandomize  Coefficient of randomize glitter scale.
 * @param [in] glitterAngleRandomize  Coefficient of randomize glitter angle.
 * @param [in] glitterApplyShape  Switch flag whether apply shape texture or not.
 * @param [in] TEXTURE2D(glitterShapeTex)  Shape texture of glitter.
 * @param [in] glitterShapeTex_ST  Tiling and Offset of glitterShapeTex.
 * @param [in] glitterAtras  Glitter atlas.
 * @return Glitter color.
 */
float3 lilCalcGlitterOpt(float2 uv, float3 normalDirection, float3 viewDirection, float3 cameraDirection, float3 lightDirection, float4 glitterParams1, float4 glitterParams2, float glitterPostContrast, float glitterSensitivity, float glitterScaleRandomize, uint glitterAngleRandomize, bool glitterApplyShape, TEXTURE2D(glitterShapeTex), float4 glitterShapeTex_ST, float4 glitterAtras)
{
    // glitterParams1
    // x: Scale, y: Scale, z: Size, w: Contrast
    // glitterParams2
    // x: Speed, y: Angle, z: Light Direction, w:

    float2 pos = uv * glitterParams1.xy;
    const float2 dd = fwidth(pos);
    const float factor = frac(sin(dot(floor(pos / floor(dd + 3.0)), float2(12.9898, 78.233))) * 46203.4357) + 0.5;
    const float2 factor2 = floor(dd + factor * 0.5);
    pos = pos / max(1.0, factor2) + glitterParams1.xy * factor2;

    float2 nearoffset;
    const float4 near = lilVoronoiOpt(pos, /* out */ nearoffset, glitterScaleRandomize);

    // Glitter
    float3 glitterNormal = abs(frac(near.xyz * 14.274 + _Time.x * glitterParams2.x) * 2.0 - 1.0);
    glitterNormal = normalize(glitterNormal * 2.0 - 1.0);
    float glitter = dot(glitterNormal, cameraDirection);
    glitter = abs(frac(glitter * glitterSensitivity + glitterSensitivity) - 0.5) * 4.0 - 1.0;
    glitter = saturate(1.0 - (glitter * glitterParams1.w + glitterParams1.w));
    glitter = pow(glitter, glitterPostContrast);
    // Circle
    glitter *= saturate((glitterParams1.z - near.w) / fwidth(near.w));
    // Angle
    const float3 halfDirection = normalize(viewDirection + lightDirection * glitterParams2.z);
    const float nh = saturate(dot(normalDirection, halfDirection));
    glitter = saturate(glitter * saturate(nh * glitterParams2.y + 1.0 - glitterParams2.y));
    // Random Color
    float3 glitterColor = glitter - glitter * frac(near.xyz * 278.436) * glitterParams2.w;
    // Shape
#ifdef LIL_FEATURE_GlitterShapeTex
    if (glitterApplyShape) {
        float2 maskUV = pos - floor(pos) - nearoffset + 0.5 - near.xy;
        maskUV = maskUV / glitterParams1.z * glitterShapeTex_ST.xy + glitterShapeTex_ST.zw;
        if (glitterAngleRandomize) {
            float si, co;
            sincos(near.z * 785.238, si, co);
            maskUV = float2(
                maskUV.x * co - maskUV.y * si,
                maskUV.x * si + maskUV.y * co);
        }
        const float randomScale = lerp(1.0, rsqrt(max(near.z, 0.001)), glitterScaleRandomize);
        maskUV = maskUV * randomScale + 0.5;
        const bool clamp = all(maskUV.xy == saturate(maskUV.xy));
        maskUV = (maskUV + floor(near.xy * glitterAtras.xy)) / glitterAtras.xy;
        const float2 mipfactor = 0.125 / glitterParams1.z * glitterAtras.xy * glitterShapeTex_ST.xy * randomScale;
        float4 shapeTex = LIL_SAMPLE_2D_GRAD(glitterShapeTex, lil_sampler_linear_clamp, maskUV, abs(ddx(pos)) * mipfactor.x, abs(ddy(pos)) * mipfactor.y);
        shapeTex.a = clamp ? shapeTex.a : 0;
        glitterColor *= shapeTex.rgb * shapeTex.a;
    }
#endif  // LIL_FEATURE_GlitterShapeTex

    return glitterColor;
}


#if !(defined(SHADER_API_D3D9) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS))
/*!
 * @brief Identify texture size is equals to screen size.
 *
 * !!NO EFFECT ON OPTIMIZATION!!
 * Intended to avoid int-float conversion...
 *
 * @param [in] TEXTURE2D(tex)  2D texture.
 * @return True if the texture size is same to screen size, otherwise false.
 */
bool IsScreenTexOpt(TEXTURE2D(tex))
{
    float2 wh;
    tex.GetDimensions(/* out */ wh.x, /* out */ wh.y);
    const float2 absDiff = abs(wh - LIL_SCREENPARAMS.xy);
    return absDiff.x + absDiff.y < 1;
}


/*!
 * @brief Identify texture size is equals to screen size.
 *
 * !!NO EFFECT ON OPTIMIZATION!!
 * Intended to avoid int-float conversion...
 *
 * @param [in] TEXTURE2D_ARRAY(tex)  2D texture array.
 * @return True if the texture size is same to screen size, otherwise false.
 */
bool IsScreenTexOpt(TEXTURE2D_ARRAY(tex))
{
    float3 whe;
    tex.GetDimensions(/* out */ whe.x, /* out */ whe.y, /* out */ whe.z);
    const float2 absDiff = abs(whe.xy - LIL_SCREENPARAMS.xy);
    return absDiff.x + absDiff.y < 1;
}


/*!
 * @brief Identify texture size is equals to screen size.
 *
 * !!NO EFFECT ON OPTIMIZATION!!
 * Intended to avoid int-float conversion...
 *
 * @param [in] TEXTURE2D(tex)  2D texture.
 * @return True if the texture size is same to screen size, otherwise false.
 */
float2 lilGetWidthAndHeightOpt(TEXTURE2D(tex))
{
    float2 wh;
    tex.GetDimensions(/* out */ wh.x, /* out */ wh.y);
    return wh;
}


/*!
 * @brief Identify texture size is equals to screen size.
 *
 * !!NO EFFECT ON OPTIMIZATION!!
 * Intended to avoid int-float conversion...
 *
 * @param [in] TEXTURE2D_ARRAY(tex)  2D texture array.
 * @return True if the texture size is same to screen size, otherwise false.
 */
float2 lilGetWidthAndHeightOpt(TEXTURE2D_ARRAY(tex))
{
    float2 wh;
    float element;
    tex.GetDimensions(/* out */ wh.x, /* out */ wh.y, /* out */ element);
    return wh;
}
#endif  // !(defined(SHADER_API_D3D9) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER)) || defined(SHADER_TARGET_SURFACE_ANALYSIS))


/*!
 * @brief Transform clip space position to screen space position.
 *
 * This function is optimized implemnetation of lilTransformCStoSSFrag() defined in lil_common_macro.hlsl.
 * Replace (positionSS.xy / LIL_SCREENPARAMS.xy) to (positionSS.xy * LIL_SCREENPARAMS.zw - positionSS.xy).
 *
 * @param [in] positionCS  Clip space position.
 * @return Screen space position.
 */
float4 lilTransformCStoSSFragOpt(float4 positionCS)
{
    float4 positionSS = float4(positionCS.xyz * positionCS.w, positionCS.w);
    positionSS.xy = positionSS.xy * LIL_SCREENPARAMS.zw - positionSS.xy;
    return positionSS;
}


/*!
 * @brief Convert clip space position to grab UV coordinate.
 *
 * This function is optimized implemnetation of lilCStoGrabUV() defined in lil_common_macro.hlsl.
 * Replace (positionCS.xy / LIL_SCREENPARAMS.xy) to (positionCS.xy * LIL_SCREENPARAMS.zw - positionCS.xy).
 *
 * @param [in] positionCS  Clip space position.
 * @return Grab UV coordinate.
 */
float2 lilCStoGrabUVOpt(float4 positionCS)
{
    float2 uvScn = positionCS.xy * LIL_SCREENPARAMS.zw - positionCS.xy;
#ifdef UNITY_SINGLE_PASS_STEREO
    uvScn.xy = TransformStereoScreenSpaceTex(uvScn.xy, 1.0);
#endif  // UNITY_SINGLE_PASS_STEREO
    return uvScn;
}


/*!
 * @brief Convert Z buffer value to linear depth.
 *
 * This function is just for overloading.
 *
 * @param [in] z  Z buffer value.
 * @return Linear eye depth.
 */
float lilLinearEyeDepthOpt(float z)
{
    // return LIL_MATRIX_P._m23 / (z - LIL_MATRIX_P._m22 / LIL_MATRIX_P._m32);
    return LIL_MATRIX_P._m23 / (z + LIL_MATRIX_P._m22);
}


/*!
 * @brief Convert Z buffer value to linear depth.
 *
 * This function is optimized implemnetation of lilLinearEyeDepth() defined in lil_common_macro.hlsl.
 * Replace (positionCS / LIL_SCREENPARAMS.xy) to (positionCS * LIL_SCREENPARAMS.zw - positionCS).
 *
 * @param [in] z  Z buffer value.
 * @param [in] positionCS  Clip space position.
 * @return Linear eye depth.
 */
float lilLinearEyeDepthOpt(float z, float2 positionCS)
{
    float2 pos = (positionCS * LIL_SCREENPARAMS.zw - positionCS) * 2.0 - 1.0;
#if UNITY_UV_STARTS_AT_TOP
    pos.y = -pos.y;
#endif  // UNITY_UV_STARTS_AT_TOP
    const float2 subTerms = LIL_MATRIX_P._m20_m21 / LIL_MATRIX_P._m00_m11 * (pos.xy + LIL_MATRIX_P._m02_m12);
    return LIL_MATRIX_P._m23 / (z + LIL_MATRIX_P._m22 - subTerms.x - subTerms.y);
}


#ifdef LIL_BRP
/*!
 * @brief Get additional lights.
 *
 * This function is optimized implemnetation of lilGetAdditionalLights() defined in lil_common_macro.hlsl.
 * Vectorize scalar operations.
 *
 * @param [in] positionWS  World space position.
 * @param [in] positionCS  Clip space position.
 * @param [in] strength  Light strength.
 * @param [in,out] lightColor  Light color.
 * @param [in,out] lightDirection  Light direction.
 * @return Additional light color.
 */
void lilGetAdditionalLightsOpt(float3 positionWS, float4 positionCS, float strength, inout float3 lightColor, inout float3 lightDirection)
{
#if defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)
    const float4 toLightX = unity_4LightPosX0 - positionWS.x;
    const float4 toLightY = unity_4LightPosY0 - positionWS.y;
    const float4 toLightZ = unity_4LightPosZ0 - positionWS.z;

    float4 lengthSq = toLightX * toLightX + 0.000001;
    lengthSq += toLightY * toLightY;
    lengthSq += toLightZ * toLightZ;

    // float4 atten = 1.0 / (1.0 + lengthSq * unity_4LightAtten0);
    const float2 tmp = float2(25.0, 0.987725) + lengthSq.xx * float2(-unity_4LightAtten0.x, unity_4LightAtten0.x);
    const float4 atten = saturate(saturate(tmp.x * 0.111375) / tmp.y) * strength;
    lightColor += unity_LightColor[0].rgb * atten.x;
    lightColor += unity_LightColor[1].rgb * atten.y;
    lightColor += unity_LightColor[2].rgb * atten.z;
    lightColor += unity_LightColor[3].rgb * atten.w;

    const float4 lar = atten * rsqrt(lengthSq) * float4(
        lilLuminance(unity_LightColor[0].rgb),
        lilLuminance(unity_LightColor[1].rgb),
        lilLuminance(unity_LightColor[2].rgb),
        lilLuminance(unity_LightColor[3].rgb));
    lightDirection += lar.x * float3(toLightX.x, toLightY.x, toLightZ.x);
    lightDirection += lar.y * float3(toLightX.y, toLightY.y, toLightZ.y);
    lightDirection += lar.z * float3(toLightX.z, toLightY.z, toLightZ.z);
    lightDirection += lar.w * float3(toLightX.w, toLightY.w, toLightZ.w);
#endif  // defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)
}


/*!
 * @brief Get additional lights.
 *
 * This function is just for overloading.
 *
 * @param [in] positionWS  World space position.
 * @param [in] positionCS  Clip space position.
 * @param [in] strength  Light strength.
 * @return Additional light color.
 */
float3 lilGetAdditionalLightsOpt(float3 positionWS, float4 positionCS, float strength)
{
    float3 lightColor = 0.0;
    float3 lightDirection = 0.0;
    lilGetAdditionalLightsOpt(positionWS, positionCS, strength, /* inout */ lightColor, /* inout */ lightDirection);
    return saturate(lightColor);
}


/*!
 * @brief Calculate color of environment reflection.
 *
 * This function is optimized implemnetation of lilGetEnvReflection() defined in lil_common_macro.hlsl.
 * Use UnityGI_IndirectSpecularOpt().
 *
 * @param [in] viewDirection  View direction.
 * @param [in] normalDirection  Normal direction.
 * @param [in] perceptualRoughness  Preceptual roughness.
 * @param [in] positionWS  World space position.
 * @return Specular color.
 * @see UnityGI_IndirectSpecularOpt
 */
float3 lilGetEnvReflectionOpt(float3 viewDirection, float3 normalDirection, float perceptualRoughness, float3 positionWS)
{
    const UnityGIInput data = lilSetupGIInput(positionWS);
    const Unity_GlossyEnvironmentData glossIn = lilSetupGlossyEnvironmentData(viewDirection, normalDirection, perceptualRoughness);
    return UnityGI_IndirectSpecularOpt(data, 1.0, glossIn);
}
#endif  // LIL_BRP


/*!
 * @brief Decodes HDR textures; handles dLDR, RGBM formats.
 *
 * This function is optimized implemnetation of DecodeHDR() defined in UnityCG.cginc.
 * "MAD"nize in the code pass of UNITY_COLORSPACE_GAMMA.
 *
 * @param [in] data  HDR color data.
 * @param [in] hdr  Decode instruction.
 * @return Decoded color data.
 */
half3 DecodeHDROpt(half4 data, half4 hdr)
{
#if defined(UNITY_COLORSPACE_GAMMA)
    return (hdr.x * (hdr.w * data.a - hdr.w) + hdr.x) * data.rgb;
#elif defined(UNITY_USE_NATIVE_HDR)
    // Multiplier for future HDRI relative to absolute conversion.
    return hdr.x * data.rgb;
#else
    const half alpha = hdr.w * (data.a - 1.0) + 1.0;
    return (hdr.x * pow(alpha, hdr.y)) * data.rgb;
#endif  // defined(UNITY_COLORSPACE_GAMMA)
}


/*!
 * @brief Obtain reflection direction considering box projection.
 *
 * This function is optimized implemnetation of BoxProjectedCubemapDirection() defined in UnityStandardUtils.cginc.
 * Reduce operations with formula transformation.
 *
 * @param [in] worldRefDir  Refrection dir (must be normalized).
 * @param [in] worldPos  World coordinate.
 * @param [in] probePos  Position of Refrection probe.
 * @param [in] boxMin  Position of Refrection probe.
 * @param [in] boxMax  Position of Refrection probe.
 * @return Refrection direction considering box projection.
 */
float3 BoxProjectedCubemapDirectionOpt(float3 worldRefDir, float3 worldPos, float4 probePos, float4 boxMin, float4 boxMax)
{
    // UNITY_SPECCUBE_BOX_PROJECTION is defined if
    // "Reflection Probes Box Projection" of GraphicsSettings is enabled.
#ifdef UNITY_SPECCUBE_BOX_PROJECTION
    // probePos.w == 1.0 if Box Projection is enabled.
    UNITY_BRANCH
    if (probePos.w > 0.0) {
        const float3 magnitudes = ((worldRefDir > float3(0.0, 0.0, 0.0) ? boxMax.xyz : boxMin.xyz) - worldPos) / worldRefDir;
        return worldRefDir * min(magnitudes.x, min(magnitudes.y, magnitudes.z)) + (worldPos - probePos);
    } else {
        return worldRefDir;
    }
#else
    return worldRefDir;
#endif  // UNITY_SPECCUBE_BOX_PROJECTION
}


/*!
 * @brief Sample and decode cube map data.
 *
 * This function is optimized implemnetation of Unity_GlossyEnvironment() defined in UnityImageBasedLighting.cginc.
 * Use DecodeHDROpt.
 *
 * @param [in] UNITY_ARGS_TEXCUBE(tex)  Sampler and texture.
 * @param [in] hdr  Decode instruction.
 * @param [in] glossIn  Glossy environment data.
 * @return Decoded sampled cube map data.
 * @see @see DecodeHDROpt
 */
half3 Unity_GlossyEnvironmentOpt(UNITY_ARGS_TEXCUBE(tex), half4 hdr, Unity_GlossyEnvironmentData glossIn)
{
    // TODO: CAUTION: remap from Morten may work only with offline convolution, see impact with runtime convolution!
    // For now disabled
#if 0
    // m is the real roughness parameter
    const float m = PerceptualRoughnessToRoughness(glossIn.roughness);

    // smallest such that 1.0 + FLT_EPSILON != 1.0  (+1e-4h is NOT good here. is visibly very wrong)
    const float fEps = 1.192092896e-07F;

    // remap to spec power. See eq. 21 in
    // --> https://dl.dropboxusercontent.com/u/55891920/papers/mm_brdf.pdf
    float n = (2.0 / max(fEps, m * m)) - 2.0;

    // remap from n_dot_h formulatino to n_dot_r.
    // See section "Pre-convolved Cube Maps vs Path Tracers"
    // --> https://s3.amazonaws.com/docs.knaldtech.com/knald/1.0.0/lys_power_drops.html
    n /= 4;

    const half perceptualRoughness = pow(2 / (n + 2), 0.25);      // remap back to square root of real roughness (0.25 include both the sqrt root of the conversion and sqrt for going from roughness to perceptualRoughness)
#else
    // MM: came up with a surprisingly close approximation to what the #if 0'ed out code above does.
    const half perceptualRoughness = glossIn.roughness * (1.7 - 0.7 * glossIn.roughness);
#endif
    const half mip = perceptualRoughnessToMipmapLevel(perceptualRoughness);
    const half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(tex, glossIn.reflUVW, mip);

    return DecodeHDROpt(rgbm, hdr);
}


/*!
 * @brief Calculate indirect specular.
 *
 * This function is optimized implemnetation of UnityGI_IndirectSpecular() defined in UnityGlobalIllumination.cginc.
 * Use DecodeHDROpt() which is "MAD"nized function.
 *
 * @param [in] data  UnityGIInput data.
 * @param [in] occlusion  Occlusion parameter.
 * @param [in] glossIn  Unity_GlossyEnvironmentData data.
 * @return Specular color.
 * @see BoxProjectedCubemapDirectionOpt
 * @see Unity_GlossyEnvironmentOpt
 */
half3 UnityGI_IndirectSpecularOpt(UnityGIInput data, half occlusion, Unity_GlossyEnvironmentData glossIn)
{
    half3 specular;

#ifdef UNITY_SPECCUBE_BOX_PROJECTION
    // we will tweak reflUVW in glossIn directly (as we pass it to Unity_GlossyEnvironment twice for probe0 and probe1),
    // so keep original to pass into BoxProjectedCubemapDirectionOpt.
    const half3 originalReflUVW = glossIn.reflUVW;
    glossIn.reflUVW = BoxProjectedCubemapDirectionOpt(originalReflUVW, data.worldPos, data.probePosition[0], data.boxMin[0], data.boxMax[0]);
#endif  // UNITY_SPECCUBE_BOX_PROJECTION

#ifdef _GLOSSYREFLECTIONS_OFF
    specular = unity_IndirectSpecColor.rgb;
#    else
    const half3 env0 = Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE(unity_SpecCube0), data.probeHDR[0], glossIn);
#    ifdef UNITY_SPECCUBE_BLENDING
        static const float kBlendFactor = 0.99999;
        float blendLerp = data.boxMin[0].w;
        UNITY_BRANCH
        if (blendLerp < kBlendFactor) {
#        ifdef UNITY_SPECCUBE_BOX_PROJECTION
            glossIn.reflUVW = BoxProjectedCubemapDirectionOpt(originalReflUVW, data.worldPos, data.probePosition[1], data.boxMin[1], data.boxMax[1]);
#        endif  // UNITY_SPECCUBE_BOX_PROJECTION
            const half3 env1 = Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1,unity_SpecCube0), data.probeHDR[1], glossIn);
            specular = lerp(env1, env0, blendLerp);
        } else {
            specular = env0;
        }
#    else
        specular = env0;
#    endif  // UNITY_SPECCUBE_BLENDING
#endif  // _GLOSSYREFLECTIONS_OFF

    return specular * occlusion;
}


#endif  // LIL_OPT_COMMON_FUNCTIONS_INCLUDED
