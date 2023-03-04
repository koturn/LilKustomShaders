#ifndef OPT_LIL_COMMON_FUNCTIONS
#define OPT_LIL_COMMON_FUNCTIONS


float3 lilDecodeHDROpt(float4 data, float4 hdr);
float3 lilCustomReflectionOpt(TEXTURECUBE(tex), float4 hdr, float3 viewDirection, float3 normalDirection, float perceptualRoughness);
float3 lilToneCorrectionOpt(float3 c, float4 hsvg);
void lilPOMOpt(inout float2 uvMain, inout float2 uv, lilBool useParallax, float4 uv_st, float3 parallaxViewDirection, TEXTURE2D(parallaxMap), float parallaxScale, float parallaxOffsetParam);
float3 lilGetEnvReflectionOpt(float3 viewDirection, float3 normalDirection, float perceptualRoughness, float3 positionWS);

half3 DecodeHDROpt(half4 data, half4 hdr);
float3 BoxProjectedCubemapDirectionOpt(float3 worldRefDir, float3 worldPos, float4 probePos, float4 boxMin, float4 boxMax);
half3 Unity_GlossyEnvironmentOpt(UNITY_ARGS_TEXCUBE(tex), half4 hdr, Unity_GlossyEnvironmentData glossIn);
half3 UnityGI_IndirectSpecularOpt(UnityGIInput data, half occlusion, Unity_GlossyEnvironmentData glossIn);


#define lilDecodeHDR lilDecodeHDROpt
#define lilCustomReflection lilCustomReflectionOpt
#define lilToneCorrection lilToneCorrectionOpt
#define lilPOM lilPOMOpt
#define lilGetEnvReflection lilGetEnvReflectionOpt

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
#endif
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
    float4 p1 = float4(b1 ? c.gb : c.bg, b1 ? k1.wz : k1.xy);

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
#endif
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
#endif

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
#    endif
#endif  // _GLOSSYREFLECTIONS_OFF

    return specular * occlusion;
}


#endif  // OPT_LIL_COMMON_FUNCTIONS
