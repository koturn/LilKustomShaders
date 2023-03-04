#ifndef OPT_LIL_COMMON_FUNCTIONS
#define OPT_LIL_COMMON_FUNCTIONS


float3 lilDecodeHDROpt(float4 data, float4 hdr);
float3 lilCustomReflectionOpt(TEXTURECUBE(tex), float4 hdr, float3 viewDirection, float3 normalDirection, float perceptualRoughness);
float3 lilToneCorrectionOpt(float3 c, float4 hsvg);
void lilPOMOpt(inout float2 uvMain, inout float2 uv, lilBool useParallax, float4 uv_st, float3 parallaxViewDirection, TEXTURE2D(parallaxMap), float parallaxScale, float parallaxOffsetParam);
float3 lilCalcGlitterOpt(float2 uv, float3 normalDirection, float3 viewDirection, float3 cameraDirection, float3 lightDirection, float4 glitterParams1, float4 glitterParams2, float glitterPostContrast, float glitterSensitivity, float glitterScaleRandomize, uint glitterAngleRandomize, bool glitterApplyShape, TEXTURE2D(glitterShapeTex), float4 glitterShapeTex_ST, float4 glitterAtras);

#ifdef LIL_BRP
void lilGetAdditionalLightsOpt(float3 positionWS, float4 positionCS, float strength, inout float3 lightColor, inout float3 lightDirection);;
float3 lilGetAdditionalLightsOpt(float3 positionWS, float4 positionCS, float strength);
float3 lilGetEnvReflectionOpt(float3 viewDirection, float3 normalDirection, float perceptualRoughness, float3 positionWS);
#endif  // LIL_BRP

half3 DecodeHDROpt(half4 data, half4 hdr);
float3 BoxProjectedCubemapDirectionOpt(float3 worldRefDir, float3 worldPos, float4 probePos, float4 boxMin, float4 boxMax);
half3 Unity_GlossyEnvironmentOpt(UNITY_ARGS_TEXCUBE(tex), half4 hdr, Unity_GlossyEnvironmentData glossIn);
half3 UnityGI_IndirectSpecularOpt(UnityGIInput data, half occlusion, Unity_GlossyEnvironmentData glossIn);


#define lilDecodeHDR lilDecodeHDROpt
#define lilCustomReflection lilCustomReflectionOpt
#define lilToneCorrection lilToneCorrectionOpt
#define lilPOM lilPOMOpt
#define lilCalcGlitter lilCalcGlitterOpt

#ifdef LIL_BRP
#    define lilGetAdditionalLights lilGetAdditionalLightsOpt
#    define lilGetEnvReflection lilGetEnvReflectionOpt
#endif  // LIL_BRP

#define DecodeHDR DecodeHDROpt
#define BoxProjectedCubemapDirection BoxProjectedCubemapDirectionOpt
#define Unity_GlossyEnvironment Unity_GlossyEnvironmentOpt
#define UnityGI_IndirectSpecular UnityGI_IndirectSpecularOpt


/*!
 * @brief Decodes HDR textures; handles dLDR, RGBM formats.
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
 * @param [in] TEXTURECUBE(tex)  Cube map texture.
 * @param [in] hdr  Decode instruction.
 * @param [in] viewDirection  View direction.
 * @param [in] normalDirection  Normal direction.
 * @param [in] perceptualRoughness  Preceptual roughness.
 * @return Decoded color data.
 */
float3 lilCustomReflectionOpt(TEXTURECUBE(tex), float4 hdr, float3 viewDirection, float3 normalDirection, float perceptualRoughness)
{
    const float mip = perceptualRoughness * (10.2 - 4.2 * perceptualRoughness);
    const float3 refl = reflect(-viewDirection, normalDirection);
    return lilDecodeHDROpt(LIL_SAMPLE_CUBE_LOD(tex, lil_sampler_linear_repeat, refl, mip), hdr);
}


/*!
 * @brief Correct color tone.
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
 * @brief Calculate Parallax Occulusion Mapping.
 * @param [in,out] uvMain  Main UV coordinate.
 * @param [in,out] uv  UV coordinate.
 * @param [in] useParallax  Switch flag whether use parallax or not.
 * @param [in] uv_st  Tiling and Offset of uv.
 * @param [in] parallaxViewDirection  Parallax view direction.
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
 * @brief Calculate glitter.
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
    const float4 near = lilVoronoi(pos, /* out */ nearoffset, glitterScaleRandomize);

    // Glitter
    float3 glitterNormal = abs(frac(near.xyz * 14.274 + _Time.x * glitterParams2.x) * 2.0 - 1.0);
    glitterNormal = normalize(glitterNormal * 2.0 - 1.0);
    float glitter = dot(glitterNormal, cameraDirection);
    glitter = abs(frac(glitter * glitterSensitivity + glitterSensitivity) - 0.5) * 4.0 - 1.0;
    glitter = saturate(1.0 - (glitter * glitterParams1.w + glitterParams1.w));
    glitter = pow(glitter, glitterPostContrast);
    // Circle
    glitter *= saturate((glitterParams1.z-near.w) / fwidth(near.w));
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


#ifdef LIL_BRP
/*!
 * @brief Get additional lights.
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
    const float4 atten = saturate(saturate((25.0 - lengthSq * unity_4LightAtten0) * 0.111375) / (0.987725 + lengthSq * unity_4LightAtten0)) * strength;
    lightColor += unity_LightColor[0].rgb * atten.x;
    lightColor += unity_LightColor[1].rgb * atten.y;
    lightColor += unity_LightColor[2].rgb * atten.z;
    lightColor += unity_LightColor[3].rgb * atten.w;

    const float4 attenLengthRcp = atten * rsqrt(lengthSq);
    lightDirection += lilLuminance(unity_LightColor[0].rgb) * attenLengthRcp.x * float3(toLightX.x, toLightY.x, toLightZ.x);
    lightDirection += lilLuminance(unity_LightColor[1].rgb) * attenLengthRcp.y * float3(toLightX.y, toLightY.y, toLightZ.y);
    lightDirection += lilLuminance(unity_LightColor[2].rgb) * attenLengthRcp.z * float3(toLightX.z, toLightY.z, toLightZ.z);
    lightDirection += lilLuminance(unity_LightColor[3].rgb) * attenLengthRcp.w * float3(toLightX.w, toLightY.w, toLightZ.w);
#endif  // defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)
}


/*!
 * @brief Get additional lights.
 * @param [in] positionWS  World space position.
 * @param [in] positionCS  Clip space position.
 * @param [in] strength  Light strength.
 * @return Additional light color.
 */
float3 lilGetAdditionalLightsOpt(float3 positionWS, float4 positionCS, float strength)
{
    float3 lightColor = 0.0;
    float3 lightDirection = 0.0;
    lilGetAdditionalLights(positionWS, positionCS, strength, /* inout */ lightColor, /* inout */ lightDirection);
    return saturate(lightColor);
}


/*!
 * @brief Calculate color of environment reflection.
 * @param [in] viewDirection  View direction.
 * @param [in] normalDirection  Normal direction.
 * @param [in] perceptualRoughness  Preceptual roughness.
 * @param [in] positionWS  World space position.
 * @return Specular color.
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
 * @param [in] UNITY_ARGS_TEXCUBE(tex)  Sampler and texture.
 * @param [in] hdr  Decode instruction.
 * @param [in] glossIn  Glossy environment data.
 * @return Decoded sampled cube map data.
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

    const half perceptualRoughness = pow( 2/(n+2), 0.25);      // remap back to square root of real roughness (0.25 include both the sqrt root of the conversion and sqrt for going from roughness to perceptualRoughness)
#else
    // MM: came up with a surprisingly close approximation to what the #if 0'ed out code above does.
    const half perceptualRoughness = glossIn.roughness * (1.7 - 0.7 * glossIn.roughness);
#endif
    const half mip = perceptualRoughnessToMipmapLevel(perceptualRoughness);
    const half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(tex, glossIn.reflUVW, mip);

    return DecodeHDR(rgbm, hdr);
}


/*!
 * @brief Calculate indirect specular.
 * @param [in] data  UnityGIInput data.
 * @param [in] occlusion  Occlusion parameter.
 * @param [in] glossIn  Unity_GlossyEnvironmentData data.
 * @return Specular color.
 */
half3 UnityGI_IndirectSpecularOpt(UnityGIInput data, half occlusion, Unity_GlossyEnvironmentData glossIn)
{
    half3 specular;

#ifdef UNITY_SPECCUBE_BOX_PROJECTION
    // we will tweak reflUVW in glossIn directly (as we pass it to Unity_GlossyEnvironment twice for probe0 and probe1),
    // so keep original to pass into BoxProjectedCubemapDirection
    const half3 originalReflUVW = glossIn.reflUVW;
    glossIn.reflUVW = BoxProjectedCubemapDirection(originalReflUVW, data.worldPos, data.probePosition[0], data.boxMin[0], data.boxMax[0]);
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
            glossIn.reflUVW = BoxProjectedCubemapDirection(originalReflUVW, data.worldPos, data.probePosition[1], data.boxMin[1], data.boxMax[1]);
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


#endif  // OPT_LIL_COMMON_FUNCTIONS
