/*!
 * @brief SDF (Signed Distance Function) of objects for albedo.
 * @param [in] p  Coordinate.
 * @return Signed Distance to the objects.
 */
float mapAlbedo(float2 p)
{
    p = invAffineTransform(p, _GraphKoturnOffsetScale.zw, _GraphKoturnRotAngle, _GraphKoturnOffsetScale.xy);
    return sdKoturn(p, float3(2.0, 10.0, 5.0), float3(0.0, 3.0, 1.5));
}


/*!
 * @brief SDF (Signed Distance Function) of objects for emission.
 * @param [in] p  Coordinate.
 * @return Signed Distance to the objects.
 */
float mapEmission(float2 p)
{
    p = invAffineTransform(p, _StarOffsetScale.zw, _StarRotAngle + LIL_TIME * _StarRotSpeed, _StarOffsetScale.xy);
    const float d = sdStar5(p, 1.25 + _SinTime.z * _StarRotSpeed, 0.385);
    return d < -_StarWidth ? -d : d;
}


/*!
 * @brief Calculate star emission
 * @param [in] fd  Fragment data.
 * @param [in] hsValue  Hue Shift Value
 */
void starEmission(inout lilFragData fd, float hsValue)
{
    const float emissionBlend = _StarColor.a;
#if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
    emissionBlend *= fd.col.a;
#endif  // LIL_RENDER == 2 && !defined(LIL_REFRACTION)
    const float3 starColor = mapEmission(fd.uvMain * 2.0 - 1.0) <= 0.0 ? rgbAddHue(_StarColor.rgb, hsValue)
        : _StarBlendMode == 1 || _StarBlendMode == 2 ? float3(0.0, 0.0, 0.0)
        : _StarBlendMode == 3 ? float3(1.0, 1.0, 1.0)
        : fd.col.rgb;
    if (_StarBlendMode > 0) {
        fd.col.rgb = lilBlendColor(fd.col.rgb, starColor.rgb, emissionBlend, _StarBlendMode);
    }
}


#if defined(LIL_FEATURE_EMISSION_1ST) && !defined(LIL_LITE)
    /*!
     * @brief lilEmission() considering Hue Shift.
     * @param [in] fd  Fragment data.
     * @param [in] samp  Sampler state.
     */
    void lilEmissionHueShift(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseEmission)
        {
            float4 emissionColor = _EmissionColor;
            // UV
            float2 emissionUV = fd.uv0;
            if(_EmissionMap_UVMode == 1) emissionUV = fd.uv1;
            if(_EmissionMap_UVMode == 2) emissionUV = fd.uv2;
            if(_EmissionMap_UVMode == 3) emissionUV = fd.uv3;
            if(_EmissionMap_UVMode == 4) emissionUV = fd.uvRim;
            //if(_EmissionMap_UVMode == 4) emissionUV = fd.uvPanorama;
            float2 _EmissionMapParaTex = emissionUV + _EmissionParallaxDepth * fd.parallaxOffset;
            // Texture
            #if defined(LIL_FEATURE_EmissionMap)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                    emissionColor *= LIL_GET_EMITEX(_EmissionMap, _EmissionMapParaTex);
                #else
                    emissionColor *= LIL_SAMPLE_2D_ST(_EmissionMap, sampler_EmissionMap, _EmissionMapParaTex);
                #endif
            #endif
            // Mask
            #if defined(LIL_FEATURE_EmissionBlendMask)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                    emissionColor *= LIL_GET_EMIMASK(_EmissionBlendMask, fd.uv0);
                #else
                    emissionColor *= LIL_SAMPLE_2D_ST(_EmissionBlendMask, samp, fd.uvMain);
                #endif
            #endif
            // Gradation
            #if defined(LIL_FEATURE_EmissionGradTex)
                #if defined(LIL_FEATURE_EMISSION_GRADATION) && defined(LIL_FEATURE_AUDIOLINK)
                    if(_EmissionUseGrad)
                    {
                        float gradUV = _EmissionGradSpeed * LIL_TIME + fd.audioLinkValue * _AudioLink2EmissionGrad;
                        emissionColor *= LIL_SAMPLE_1D_LOD(_EmissionGradTex, lil_sampler_linear_repeat, gradUV, 0);
                    }
                #elif defined(LIL_FEATURE_EMISSION_GRADATION)
                    if(_EmissionUseGrad) emissionColor *= LIL_SAMPLE_1D(_EmissionGradTex, lil_sampler_linear_repeat, _EmissionGradSpeed * LIL_TIME);
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission) emissionColor.a *= fd.audioLinkValue;
            #endif
            emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * fd.invLighting, _EmissionFluorescence);
            emissionColor.rgb = lerp(emissionColor.rgb, emissionColor.rgb * fd.albedo, _EmissionMainStrength);
            float emissionBlend = _EmissionBlend * lilCalcBlink(_EmissionBlink) * emissionColor.a;
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                emissionBlend *= fd.col.a;
            #endif
            if(_HueShiftEmission)
            {
                const float hueShiftValue = _Time.y * _HueShiftSpeed * LIL_SAMPLE_2D(_HueShiftMask, sampler_MainTex, fd.uvMain).r;
                emissionColor.rgb = rgbAddHue(emissionColor.rgb, hueShiftValue);
            }
            fd.col.rgb = lilBlendColor(fd.col.rgb, emissionColor.rgb, emissionBlend, _EmissionBlendMode);
        }
    }
#elif defined(LIL_LITE)
    /*!
     * @brief lilEmission() considering Hue Shift.
     * @param [in] fd  Fragment data.
     */
    void lilEmissionHueShift(inout lilFragData fd)
    {
        if(_UseEmission)
        {
            float emissionBlinkSeq = lilCalcBlink(_EmissionBlink);
            float4 emissionColor = _EmissionColor;
            float2 emissionUV = fd.uv0;
            if(_EmissionMap_UVMode == 1) emissionUV = fd.uv1;
            if(_EmissionMap_UVMode == 2) emissionUV = fd.uv2;
            if(_EmissionMap_UVMode == 3) emissionUV = fd.uv3;
            if(_EmissionMap_UVMode == 4) emissionUV = fd.uvRim;
            emissionColor *= LIL_GET_EMITEX(_EmissionMap,emissionUV);
            if(_HueShiftEmission)
            {
                const float hueShiftValue = _Time.y * _HueShiftSpeed * LIL_SAMPLE_2D(_HueShiftMask, sampler_MainTex, fd.uvMain).r;
                emissionColor.rgb = rgbAddHue(emissionColor.rgb, hueShiftValue);
            }
            fd.emissionColor += emissionBlinkSeq * fd.triMask.b * emissionColor.rgb;
        }
    }
#endif  // defined(LIL_FEATURE_EMISSION_1ST) && !defined(LIL_LITE)
#if !defined(OVERRIDE_EMISSION_1ST)
    #if defined(LIL_LITE)
        #define OVERRIDE_EMISSION_1ST \
            lilEmissionHueShift(fd);
    #else
        #define OVERRIDE_EMISSION_1ST \
            lilEmissionHueShift(fd LIL_SAMP_IN(sampler_MainTex));
    #endif  // defined(LIL_LITE)
#endif  // !defined(OVERRIDE_EMISSION_1ST)


#if defined(LIL_FEATURE_EMISSION_2ND) && !defined(LIL_LITE)
    /*!
     * @brief lilEmission2nd() considering Hue Shift.
     * @param [in] fd  Fragment data.
     * @param [in] samp  Sampler state.
     */
    void lilEmission2ndHueShift(inout lilFragData fd LIL_SAMP_IN_FUNC(samp))
    {
        if(_UseEmission2nd)
        {
            float4 emission2ndColor = _Emission2ndColor;
            // UV
            float2 emission2ndUV = fd.uv0;
            if(_Emission2ndMap_UVMode == 1) emission2ndUV = fd.uv1;
            if(_Emission2ndMap_UVMode == 2) emission2ndUV = fd.uv2;
            if(_Emission2ndMap_UVMode == 3) emission2ndUV = fd.uv3;
            if(_Emission2ndMap_UVMode == 4) emission2ndUV = fd.uvRim;
            //if(_Emission2ndMap_UVMode == 4) emission2ndUV = fd.uvPanorama;
            float2 _Emission2ndMapParaTex = emission2ndUV + _Emission2ndParallaxDepth * fd.parallaxOffset;
            // Texture
            #if defined(LIL_FEATURE_Emission2ndMap)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_UV)
                    emission2ndColor *= LIL_GET_EMITEX(_Emission2ndMap, _Emission2ndMapParaTex);
                #else
                    emission2ndColor *= LIL_SAMPLE_2D_ST(_Emission2ndMap, sampler_Emission2ndMap, _Emission2ndMapParaTex);
                #endif
            #endif
            // Mask
            #if defined(LIL_FEATURE_Emission2ndBlendMask)
                #if defined(LIL_FEATURE_ANIMATE_EMISSION_MASK_UV)
                    emission2ndColor *= LIL_GET_EMIMASK(_Emission2ndBlendMask, fd.uv0);
                #else
                    emission2ndColor *= LIL_SAMPLE_2D_ST(_Emission2ndBlendMask, samp, fd.uvMain);
                #endif
            #endif
            // Gradation
            #if defined(LIL_FEATURE_Emission2ndGradTex)
                #if defined(LIL_FEATURE_EMISSION_GRADATION) && defined(LIL_FEATURE_AUDIOLINK)
                    if(_Emission2ndUseGrad)
                    {
                        float gradUV = _Emission2ndGradSpeed * LIL_TIME + fd.audioLinkValue * _AudioLink2Emission2ndGrad;
                        emission2ndColor *= LIL_SAMPLE_1D_LOD(_Emission2ndGradTex, lil_sampler_linear_repeat, gradUV, 0);
                    }
                #elif defined(LIL_FEATURE_EMISSION_GRADATION)
                    if(_Emission2ndUseGrad) emission2ndColor *= LIL_SAMPLE_1D(_Emission2ndGradTex, lil_sampler_linear_repeat, _Emission2ndGradSpeed * LIL_TIME);
                #endif
            #endif
            #if defined(LIL_FEATURE_AUDIOLINK)
                if(_AudioLink2Emission2nd) emission2ndColor.a *= fd.audioLinkValue;
            #endif
            emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * fd.invLighting, _Emission2ndFluorescence);
            emission2ndColor.rgb = lerp(emission2ndColor.rgb, emission2ndColor.rgb * fd.albedo, _Emission2ndMainStrength);
            if(_HueShiftEmission2nd)
            {
                const float hueShiftValue = _Time.y * _HueShiftSpeed * LIL_SAMPLE_2D(_HueShiftMask, sampler_MainTex, fd.uvMain).r;
                emission2ndColor.rgb = rgbAddHue(emission2ndColor.rgb, hueShiftValue);
            }
            float emission2ndBlend = _Emission2ndBlend * lilCalcBlink(_Emission2ndBlink) * emission2ndColor.a;
            #if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
                emission2ndBlend *= fd.col.a;
            #endif
            fd.col.rgb = lilBlendColor(fd.col.rgb, emission2ndColor.rgb, emission2ndBlend, _Emission2ndBlendMode);
        }
    }
#endif  // defined(LIL_FEATURE_EMISSION_2ND) && !defined(LIL_LITE)
#if !defined(OVERRIDE_EMISSION_2ND)
    #define OVERRIDE_EMISSION_2ND \
        lilEmission2ndHueShift(fd LIL_SAMP_IN(sampler_MainTex));
#endif  // !defined(OVERRIDE_EMISSION_2ND)


#include "lil_opt_common_functions.hlsl"
#include "lil_override.hlsl"
