#ifndef LIL_OVERRIDE_INCLUDED
#define LIL_OVERRIDE_INCLUDED


#if defined(LIL_HDRP)
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_VAR , posInput
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS , PositionInputs posInput
#else
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_VAR
#    define LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS
#endif


#if defined(LIL_REFRACTION) && !defined(LIL_LITE)
void lilRefractionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
#endif  // defined(LIL_REFRACTION) && !defined(LIL_LITE)
#if defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE)
float3 lilCalcSpecularOverride(inout lilFragData fd, float3 L, float3 specular, float attenuation LIL_SAMP_IN_FUNC(samp));
void lilReflectionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp) LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS);
#endif  // defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE)


#ifndef OVERRIDE_REFRACTION
#    define OVERRIDE_REFRACTION \
        lilRefractionOverride(fd LIL_SAMP_IN(sampler_MainTex));
#endif  // !OVERRIDE_REFRACTION
#if !defined(OVERRIDE_REFLECTION)
#    define OVERRIDE_REFLECTION \
         lilReflectionOverride(fd LIL_SAMP_IN(sampler_MainTex) LIL_OVERRIDE_HDRP_POSITION_INPUT_VAR);
#endif  // !OVERRIDE_REFLECTION


#if defined(LIL_REFRACTION) && !defined(LIL_LITE)
/*!
 * @brief Calculate refraction.
 * @param [in,out] lilFragData  Fragment shader data.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Sampler stete (Optional argument).
 */
void lilRefractionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
{
    const float2 refractUV = fd.uvScn + (pow(1.0 - fd.nv, _RefractionFresnelPower) * _RefractionStrength) * mul((float3x3)LIL_MATRIX_V, fd.N).xy;
#if defined(LIL_REFRACTION_BLUR2)
#    if defined(LIL_BRP)
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
#    endif
#else
    float3 refractCol = LIL_GET_BG_TEX(refractUV,0).rgb * _RefractionColor.rgb;
#endif
    if (_RefractionColorFromMain) {
        refractCol *= fd.albedo;
    }
    fd.col.rgb = lerp(refractCol, fd.col.rgb, fd.col.a);
}
#endif


#if defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE)
/*!
 * @brief Calculate specular.
 * @param [in,out] lilFragData  Fragment shader data.
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
#endif

    // Half direction
    const float3 H = normalize(fd.V + L);
    const float nh = saturate(dot(N, H));

    // Toon
#if defined(LIL_FEATURE_ANISOTROPY)
    const bool isAnisotropy = _UseAnisotropy && _Anisotropy2Reflection;
    if (_SpecularToon & !isAnisotropy)
#else
    if (_SpecularToon)
#endif
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
#if defined(LIL_FEATURE_ANISOTROPY)
    if (isAnisotropy) {
        // const float roughnessT = max(fd.roughness * (1.0 + fd.anisotropy), 0.002);
        // const float roughnessB = max(fd.roughness * (1.0 - fd.anisotropy), 0.002);
        const float roughnessT = max(fd.roughness + fd.roughness * fd.anisotropy, 0.002);
        const float roughnessB = max(fd.roughness - fd.roughness * fd.anisotropy, 0.002);

        lambdaV = nl * length(float3(roughnessT * dot(fd.T, fd.V), roughnessB * dot(fd.B, fd.V), nv));
        lambdaL = nv * length(float3(roughnessT * dot(fd.T, L), roughnessB * dot(fd.B, L), nl));

        const float roughnessT1 = roughnessT * _AnisotropyTangentWidth;
        const float roughnessB1 = roughnessB * _AnisotropyBitangentWidth;
        const float roughnessT2 = roughnessT * _Anisotropy2ndTangentWidth;
        const float roughnessB2 = roughnessB * _Anisotropy2ndBitangentWidth;

#if defined(LIL_FEATURE_AnisotropyShiftNoiseMask)
        const float anisotropyShiftNoise = LIL_SAMPLE_2D_ST(_AnisotropyShiftNoiseMask, samp, fd.uvMain).r - 0.5;
#else
        const float anisotropyShiftNoise = 0.5;
#endif
        const float anisotropyShift = anisotropyShiftNoise * _AnisotropyShiftNoiseScale + _AnisotropyShift;
        const float anisotropy2ndShift = anisotropyShiftNoise * _Anisotropy2ndShiftNoiseScale + _Anisotropy2ndShift;

        const float r1 = roughnessT1 * roughnessB1;
        const float r2 = roughnessT2 * roughnessB2;
        const float3 v1 = float3(dot(normalize(fd.T - N * anisotropyShift), H) * roughnessB1, dot(normalize(fd.B - N * anisotropyShift), H) * roughnessT1, nh * r1);
        const float3 v2 = float3(dot(normalize(fd.T - N * anisotropy2ndShift), H) * roughnessB2, dot(normalize(fd.B - N * anisotropy2ndShift), H) * roughnessT2, nh * r2);
        const float w1 = r1 / dot(v1, v1);
        const float w2 = r2 / dot(v2, v2);
        ggx = r1 * w1 * w1 * _AnisotropySpecularStrength + r2 * w2 * w2 * _Anisotropy2ndSpecularStrength;
    }
    else
#endif
    {
        const float roughness2 = max(fd.roughness, 0.002);
        // lambdaV = nl * (nv * (1.0 - roughness2) + roughness2);
        // lambdaL = nv * (nl * (1.0 - roughness2) + roughness2);
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
#endif

    float specularTerm = sjggx * ggx;
#ifdef LIL_COLORSPACE_GAMMA
    specularTerm = sqrt(max(1e-4h, specularTerm));
#endif
    specularTerm *= nl * attenuation;

    // Output
#if defined(LIL_FEATURE_ANISOTROPY)
    if (_SpecularToon) {
        return lilTooningScale(_AAStrength, specularTerm, 0.5);
    }
#endif

    const float lh = saturate(dot(L, H));
    return specularTerm * lilFresnelTerm(specular, lh);
}


/*!
 * @brief Calculate reflection.
 * @param [in,out] lilFragData  Fragment shader data.
 * @param [in] LIL_SAMP_IN_FUNC(samp)  Sampler stete (Optional argument).
 * @param [in] LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS  Position inputs.
 */
void lilReflectionOverride(inout lilFragData fd LIL_SAMP_IN_FUNC(samp) LIL_OVERRIDE_HDRP_POSITION_INPUT_ARGS)
{
#if defined(LIL_PASS_FORWARDADD)
    if (_UseReflection && _ApplySpecular && _ApplySpecularFA)
#else
    if (_UseReflection)
#endif  // defined(LIL_PASS_FORWARDADD)
    {
        // Smoothness
#if !defined(LIL_REFRACTION_BLUR2) || defined(LIL_PASS_FORWARDADD)
        fd.smoothness = _Smoothness;
#    if defined(LIL_FEATURE_SmoothnessTex)
        fd.smoothness *= LIL_SAMPLE_2D_ST(_SmoothnessTex, samp, fd.uvMain).r;
#    endif  // defined(LIL_FEATURE_SmoothnessTex)
        GSAAForSmoothness(fd.smoothness, fd.N, _GSAAStrength);
        fd.perceptualRoughness = fd.perceptualRoughness - fd.smoothness * fd.perceptualRoughness;
        fd.roughness = fd.perceptualRoughness * fd.perceptualRoughness;
#endif
        // Metallic
        float metallic = _Metallic;
#if defined(LIL_FEATURE_MetallicGlossMap)
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
#if !defined(LIL_PASS_FORWARDADD)
        if (_ApplySpecular)
#endif
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
#if !defined(LIL_PASS_FORWARDADD)
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
#endif  //  !defined(LIL_PASS_FORWARDADD)
    }
}
#endif  //  defined(LIL_FEATURE_REFLECTION) && !defined(LIL_LITE)


#endif  // LIL_OVERRIDE_INCLUDED
