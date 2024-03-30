//----------------------------------------------------------------------------------------------------------------------
// Macro
#include "lil_current_version.hlsl"

// Custom variables
//#define LIL_CUSTOM_PROPERTIES \
//    float _CustomVariable;
#define LIL_CUSTOM_PROPERTIES \
    float _WireframeWidth; \
    float4 _WireframeColor; \
    bool _WireframeRandomizeColor; \
    float _WireframeCycleTime; \
    float _WireframeDecayTime;

// Custom textures
#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_WireframeMask);

// Add vertex shader input
//#define LIL_REQUIRE_APP_POSITION
//#define LIL_REQUIRE_APP_TEXCOORD0
//#define LIL_REQUIRE_APP_TEXCOORD1
//#define LIL_REQUIRE_APP_TEXCOORD2
//#define LIL_REQUIRE_APP_TEXCOORD3
//#define LIL_REQUIRE_APP_TEXCOORD4
//#define LIL_REQUIRE_APP_TEXCOORD5
//#define LIL_REQUIRE_APP_TEXCOORD6
//#define LIL_REQUIRE_APP_TEXCOORD7
//#define LIL_REQUIRE_APP_COLOR
//#define LIL_REQUIRE_APP_NORMAL
//#define LIL_REQUIRE_APP_TANGENT
#define LIL_REQUIRE_APP_VERTEXID

// Add vertex shader output
//#define LIL_V2F_FORCE_TEXCOORD0
//#define LIL_V2F_FORCE_TEXCOORD1
//#define LIL_V2F_FORCE_POSITION_OS
//#define LIL_V2F_FORCE_POSITION_WS
//#define LIL_V2F_FORCE_POSITION_SS
//#define LIL_V2F_FORCE_NORMAL
//#define LIL_V2F_FORCE_TANGENT
//#define LIL_V2F_FORCE_BITANGENT
#if LIL_CURRENT_VERSION_VALUE == 34 && defined(UNITY_PASS_SHADOWCASTER)
// Work around for the following bug in lilxyzw/lilToon ver.1.4.0:
//   https://github.com/lilxyzw/lilToon/issues/98
// Fixed in lilxyzw/lilToon ver.1.4.1:
//   https://github.com/lilxyzw/lilToon/commit/a8548792c56537575bb2933d65233c8c9bdca4de
#    define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
        float3 baryCoord : TEXCOORD ## id1; \
        nointerpolation float3 color0 : TEXCOORD ## id2; \
        nointerpolation float3 color1 : TEXCOORD ## id3; \
        nointerpolation float3 color2 : TEXCOORD ## id4; \
        nointerpolation float3 emissionWeights : TEXCOORD ## id5;
#else
#    define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
        float3 baryCoord : TEXCOORD ## id0; \
        nointerpolation float3 color0 : TEXCOORD ## id1; \
        nointerpolation float3 color1 : TEXCOORD ## id2; \
        nointerpolation float3 color2 : TEXCOORD ## id3; \
        nointerpolation float3 emissionWeights : TEXCOORD ## id4;
#endif

// Add vertex copy
#define LIL_CUSTOM_VERT_COPY \
    LIL_V2F_OUT.baryCoord.x = (float)input.vertexID;

// Inserting a process into the vertex shader
//#define LIL_CUSTOM_VERTEX_OS
//#define LIL_CUSTOM_VERTEX_WS

// Inserting a process into pixel shader
//#define BEFORE_xx
//#define OVERRIDE_xx
#define BEFORE_BLEND_EMISSION \
    const float3 emissionWeights = (input.baryCoord < _WireframeWidth) ? input.emissionWeights : (0.0).xxx; \
    const float emissionWeight = max(emissionWeights.x, max(emissionWeights.y, emissionWeights.z)); \
    const float3 emissionColor = emissionWeight == emissionWeights.x ? input.color0 \
        : emissionWeight == emissionWeights.y ? input.color1 \
        : input.color2; \
    fd.col.rgb += calcEmissionColor(LIL_SAMPLE_2D(_WireframeMask, sampler_MainTex, fd.uvMain) * emissionColor * emissionWeight, fd.col.a);

//----------------------------------------------------------------------------------------------------------------------
// Information about variables
//----------------------------------------------------------------------------------------------------------------------

//----------------------------------------------------------------------------------------------------------------------
// Vertex shader inputs (appdata structure)
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   input.positionOS        POSITION
// float2   input.uv0               TEXCOORD0
// float2   input.uv1               TEXCOORD1
// float2   input.uv2               TEXCOORD2
// float2   input.uv3               TEXCOORD3
// float2   input.uv4               TEXCOORD4
// float2   input.uv5               TEXCOORD5
// float2   input.uv6               TEXCOORD6
// float2   input.uv7               TEXCOORD7
// float4   input.color             COLOR
// float3   input.normalOS          NORMAL
// float4   input.tangentOS         TANGENT
// uint     vertexID                SV_VertexID

//----------------------------------------------------------------------------------------------------------------------
// Vertex shader outputs or pixel shader inputs (v2f structure)
//
// The structure depends on the pass.
// Please check lil_pass_xx.hlsl for details.
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   output.positionCS       SV_POSITION
// float2   output.uv01             TEXCOORD0 TEXCOORD1
// float2   output.uv23             TEXCOORD2 TEXCOORD3
// float3   output.positionOS       object space position
// float3   output.positionWS       world space position
// float3   output.normalWS         world space normal
// float4   output.tangentWS        world space tangent

//----------------------------------------------------------------------------------------------------------------------
// Variables commonly used in the forward pass
//
// These are members of `lilFragData fd`
//
// Type     Name                    Description
// -------- ----------------------- --------------------------------------------------------------------
// float4   col                     lit color
// float3   albedo                  unlit color
// float3   emissionColor           color of emission
// -------- ----------------------- --------------------------------------------------------------------
// float3   lightColor              color of light
// float3   indLightColor           color of indirectional light
// float3   addLightColor           color of additional light
// float    attenuation             attenuation of light
// float3   invLighting             saturate((1.0 - lightColor) * sqrt(lightColor));
// -------- ----------------------- --------------------------------------------------------------------
// float2   uv0                     TEXCOORD0
// float2   uv1                     TEXCOORD1
// float2   uv2                     TEXCOORD2
// float2   uv3                     TEXCOORD3
// float2   uvMain                  Main UV
// float2   uvMat                   MatCap UV
// float2   uvRim                   Rim Light UV
// float2   uvPanorama              Panorama UV
// float2   uvScn                   Screen UV
// bool     isRightHand             input.tangentWS.w > 0.0;
// -------- ----------------------- --------------------------------------------------------------------
// float3   positionOS              object space position
// float3   positionWS              world space position
// float4   positionCS              clip space position
// float4   positionSS              screen space position
// float    depth                   distance from camera
// -------- ----------------------- --------------------------------------------------------------------
// float3x3 TBN                     tangent / bitangent / normal matrix
// float3   T                       tangent direction
// float3   B                       bitangent direction
// float3   N                       normal direction
// float3   V                       view direction
// float3   L                       light direction
// float3   origN                   normal direction without normal map
// float3   origL                   light direction without sh light
// float3   headV                   middle view direction of 2 cameras
// float3   reflectionN             normal direction for reflection
// float3   matcapN                 normal direction for reflection for MatCap
// float3   matcap2ndN              normal direction for reflection for MatCap 2nd
// float    facing                  VFACE
// -------- ----------------------- --------------------------------------------------------------------
// float    vl                      dot(viewDirection, lightDirection);
// float    hl                      dot(headDirection, lightDirection);
// float    ln                      dot(lightDirection, normalDirection);
// float    nv                      saturate(dot(normalDirection, viewDirection));
// float    nvabs                   abs(dot(normalDirection, viewDirection));
// -------- ----------------------- --------------------------------------------------------------------
// float4   triMask                 TriMask (for lite version)
// float3   parallaxViewDirection   mul(tbnWS, viewDirection);
// float2   parallaxOffset          parallaxViewDirection.xy / (parallaxViewDirection.z+0.5);
// float    anisotropy              strength of anisotropy
// float    smoothness              smoothness
// float    roughness               roughness
// float    perceptualRoughness     perceptual roughness
// float    shadowmix               this variable is 0 in the shadow area
// float    audioLinkValue          volume acquired by AudioLink
// -------- ----------------------- --------------------------------------------------------------------
// uint     renderingLayers         light layer of object (for URP / HDRP)
// uint     featureFlags            feature flags (for HDRP)
// uint2    tileIndex               tile index (for HDRP)


/*!
 * @brief Returns a random value between 0.0 and 1.0.
 * @param [in] x  First seed value vector used for generation.
 * @param [in] y  Second seed value vector used for generation.
 * @return Pseudo-random number value between 0.0 and 1.0.
 */
float3 rand(float3 x, float3 y)
{
    return frac(sin(x * 12.9898 + y * 78.233) * 43758.5453);
}


/*!
 * @brief Convert from RGB to HSV.
 * @param [in] rgb  Vector of RGB components.
 * @return Vector of HSV components.
 */
float3 rgb2hsv(float3 rgb)
{
    static const float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    static const float e = 1.0e-10;
#if 1
    // Optimized version.
    const bool b1 = rgb.g < rgb.b;
    float4 p = float4(b1 ? rgb.bg : rgb.gb, b1 ? k.wz : k.xy);

    const bool b2 = rgb.r < p.x;
    p.xyz = b2 ? p.xyw : p.yzx;
    const float4 q = b2 ? float4(p.xyz, rgb.r) : float4(rgb.r, p.xyz);

    const float d = q.x - min(q.w, q.y);
    const float2 hs = float2(q.w - q.y, d) / float2(6.0 * d + e, q.x + e);

    return float3(abs(q.z + hs.x), hs.y, q.x);
#else
    // Original version
    const float4 p = rgb.g < rgb.b ? float4(rgb.bg, k.wz) : float4(rgb.gb, k.xy);
    const float4 q = rgb.r < p.x ? float4(p.xyw, rgb.r) : float4(rgb.r, p.yzx);
    const float d = q.x - min(q.w, q.y);

    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
#endif
}


/*!
 * @brief Convert from HSV to RGB.
 * @param [in] hsv  Vector of HSV components.
 * @return Vector of RGB components.
 */
float3 hsv2rgb(float3 hsv)
{
    static const float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);

    const float3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
    return hsv.z * lerp(k.xxx, saturate(p - k.xxx), hsv.y);
}


/*!
 * @brief Add hue to RGB color.
 * @param [in] rgb  Vector of RGB components.
 * @param [in] hue  Offset of Hue.
 * @return Vector of RGB components.
 */
float3 rgbAddHue(float3 rgb, float hue)
{
    float3 hsv = rgb2hsv(rgb);
    hsv.x += hue;
    return hsv2rgb(hsv);
}
