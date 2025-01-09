//----------------------------------------------------------------------------------------------------------------------
// Macro

// Custom variables
//#define LIL_CUSTOM_PROPERTIES \
//    float _CustomVariable;
#define LIL_CUSTOM_PROPERTIES \
    float4 _GraphKoturnColor; \
    float4 _GraphKoturnOffsetScale; \
    float _GraphKoturnRotAngle; \
    uint _StarBlendMode; \
    float4 _StarColor; \
    float4 _StarOffsetScale; \
    float _StarRotAngle; \
    float _StarRotSpeed; \
    float _StarWidth; \
    float _HueShiftSpeed; \
    lilBool _HueShiftEmission; \
    lilBool _HueShiftEmission2nd;

// Custom textures
#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_HueShiftMask);

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
//#define LIL_REQUIRE_APP_VERTEXID

// Add vertex shader output
//#define LIL_V2F_FORCE_TEXCOORD0
//#define LIL_V2F_FORCE_TEXCOORD1
//#define LIL_V2F_FORCE_POSITION_OS
//#define LIL_V2F_FORCE_POSITION_WS
//#define LIL_V2F_FORCE_POSITION_SS
//#define LIL_V2F_FORCE_NORMAL
//#define LIL_V2F_FORCE_TANGENT
//#define LIL_V2F_FORCE_BITANGENT
//#define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7)

// Add vertex copy
#define LIL_CUSTOM_VERT_COPY

// Inserting a process into the vertex shader
//#define LIL_CUSTOM_VERTEX_OS
//#define LIL_CUSTOM_VERTEX_WS

// Inserting a process into pixel shader
//#define BEFORE_xx
//#define OVERRIDE_xx
#define BEFORE_SHADOW \
    const float2 suv = fd.uvMain * 2.0 - 1.0; \
    const float hsValue = _Time.y * _HueShiftSpeed * LIL_SAMPLE_2D(_HueShiftMask, sampler_MainTex, fd.uvMain).r; \
    fd.col.rgb *= mapAlbedo(suv) <= 0.0 ? _GraphKoturnColor : float3(1.0, 1.0, 1.0); \
    fd.col.rgb = rgbAddHue(fd.col.rgb, hsValue); \
    fd.albedo = fd.col.rgb;

#define BEFORE_BLEND_EMISSION \
    starEmission(fd, hsValue);


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
 * @brief Rotate on 2D plane
 * @param [in] p  Target position.
 * @param [in] angle  Angle of rotation.
 * @return Rotated vector.
 */
float2 rotate2D(float2 p, float angle)
{
    float s, c;
    sincos(angle, s, c);
    return float2(
        p.x * c - p.y * s,
        p.x * s + p.y * c);
}


/*!
 * @brief Inverse affine trasform UV coordinate.
 * @param [in] p  Coordinate [-1, 1].
 * @param [in] scale  Scaling factor.
 * @param [in] rotAngle  Rotation angle in degrees.
 * @param [in] translate Translate factor.
 * @return Affine transformed UV coordinate.
 */
float2 invAffineTransform(float2 p, float2 scale, float rotAngle, float2 translate)
{
    // translate -> rotate -> scale
    return rotate2D(p - translate, -rotAngle) / scale;
}


/*!
 * @brief SDF of Mark of Koturn.
 * @param [in] p  Coordinate.
 * @param [in] coeffsA  Coefficient vector A.
 * @param [in] coeffsB  Coefficient vector B.
 * @return Signed Distance to the Sphere.
 */
float sdKoturn(float2 p, float3 coeffsA, float3 coeffsB)
{
    p *= 5.0;

    const float2 pp = p * p;
    const float sumXY = p.x + p.y;
    const float a = abs(pp.x + pp.y - coeffsA.x * sumXY - coeffsA.y) + sumXY - coeffsA.z;

    const float2 absP = abs(p);
    const float b = max(absP.x, absP.y - coeffsB.x) + (coeffsB.y * abs(absP.x - absP.y) - coeffsB.z);

    return a / b;
}


/*!
 * @brief SDF of star 5.
 * @param [in] p  Coordinate.
 * @param [in] size  Size of the star.
 * @param [in] vd  Vary depth of the star.
 * @return Signed Distance to the objects.
 */
float sdStar5(float2 p, float size, float vd)
{
    static const float2 k1 = float2(0.809016994375, -0.587785252292);
    static const float2 k2 = float2(-k1.x, k1.y);

    p.x = abs(p.x);
    p -= 2.0 * max(dot(k1, p), 0.0) * k1;
    p -= 2.0 * max(dot(k2, p), 0.0) * k2;
    p.x = abs(p.x);
    p.y -= size;

    const float2 ba = vd * float2(-k1.y, k1.x) - float2(0.0, 1.0);
    const float h = clamp(dot(p, ba) / dot(ba, ba), 0.0, size);

    return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
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
