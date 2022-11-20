//----------------------------------------------------------------------------------------------------------------------
// Macro

// Custom variables
//#define LIL_CUSTOM_PROPERTIES \
//    float _CustomVariable;
#define LIL_CUSTOM_PROPERTIES \
    bool _EnableElapsedTime; \
    float4 _ElapsedTimeColor; \
    float4 _ElapsedTimeOffsetScale; \
    float _ElapsedTimeRotAngle; \
    float _ElapsedTimeDisplayLength; \
    float _ElapsedTimeAlign; \
    bool _EnableALTimeOfDay; \
    float4 _ALTimeOfDayColor; \
    float4 _ALTimeOfDayOffsetScale; \
    float _ALTimeOfDayRotAngle; \
    float _ALTimeOfDayDisplayLength; \
    float _ALTimeOfDayAlign; \
    float _ALTimeOfDayOffsetSeconds; \
    bool _EnableFramerate; \
    float4 _FramerateColor; \
    float4 _FramerateOffsetScale; \
    float _FramerateRotAngle; \
    float _FramerateDisplayLength; \
    float _FramerateAlign; \
    bool _EnableWorldPos; \
    float4 _WorldPosColorX; \
    float4 _WorldPosColorY; \
    float4 _WorldPosColorZ; \
    float4 _WorldPosOffsetScale; \
    float _WorldPosRotAngle; \
    float _WorldPosDisplayLength; \
    float _WorldPosAlign;

// Custom textures
#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_SpriteTex); \
    SAMPLER(sampler_SpriteTex);

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
#define BEFORE_BLEND_EMISSION \
    if (isElapsedTimeEnabled()) { \
        const float2 uv2 = invAffineTransform(fd.uvMain, _ElapsedTimeOffsetScale.zw, _ElapsedTimeRotAngle, _ElapsedTimeOffsetScale.xy); \
        if (hmul(step(0.0, uv2) * step(uv2, 1.0)) != 0.0) { \
            const float3 hms = fmodglsl( \
                LIL_TIME.xxx / float3(3600.0, 60.0, 1.0), \
                float3(100.0, 60.0, 60.0)); \
            const float hmsval = dot(floor(hms), float3(10000.0, 100.0, 1.0)); \
            fd.emissionColor += sampleSplite(hmsval, uv2, _ElapsedTimeDisplayLength, _ElapsedTimeAlign) * _ElapsedTimeColor.rgb; \
        } \
    } \
    if (isALTimeOfDayEnabled() && AudioLinkIsAvailable()) { \
        const float2 uv2 = invAffineTransform(fd.uvMain, _ALTimeOfDayOffsetScale.zw, _ALTimeOfDayRotAngle, _ALTimeOfDayOffsetScale.xy); \
        if (hmul(step(0.0, uv2) * step(uv2, 1.0)) != 0.0) { \
            const float hmsval = dot(AudioLinkGetTimeOfDay(), float3(10000.0, 100.0, 1.0)); \
            fd.emissionColor += sampleSplite(hmsval, uv2, _ALTimeOfDayDisplayLength, _ALTimeOfDayAlign) * _ALTimeOfDayColor.rgb; \
        } \
    } \
    if (isFramerateEnabled()) { \
        const float2 uv2 = invAffineTransform(fd.uvMain, _FramerateOffsetScale.zw, _FramerateRotAngle, _FramerateOffsetScale.xy); \
        if (hmul(step(0.0, uv2) * step(uv2, 1.0)) != 0.0) { \
            fd.emissionColor += sampleSplite(round(unity_DeltaTime.w), uv2, _FramerateDisplayLength, _FramerateAlign) * _FramerateColor.rgb; \
        } \
    } \
    if (isWorldPosEnabled()) { \
        float2 uv2 = invAffineTransform(fd.uvMain, _WorldPosOffsetScale.zw, _WorldPosRotAngle, _WorldPosOffsetScale.xy); \
        if (hmul(step(0.0, uv2) * step(uv2, 1.0)) != 0.0) { \
            const float3 worldPos = unity_ObjectToWorld._m03_m13_m23; \
            const float pos = round(uv2.y < (1.0 / 3.0) ? worldPos.x : uv2.y < (2.0 / 3.0) ? worldPos.y : worldPos.z); \
            const float3 posCol = uv2.y < (1.0 / 3.0) ? _WorldPosColorX : uv2.y < (2.0 / 3.0) ? _WorldPosColorY : _WorldPosColorZ; \
            uv2.y *= 3.0; \
            fd.emissionColor += sampleSpliteSigned(pos, uv2, _WorldPosDisplayLength, _WorldPosAlign) * posCol; \
        } \
    }


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
 * @brief Remap x from [a, b] to [0, 1].
 * @param [in] a  Source min value.
 * @param [in] b  Source max value.
 * @param [in] x  Remap target value.
 * @return Remapped value.
 */
float remap01(float a, float b, float x)
{
    return (x - a) / (b - a);
}


/*!
 * @brief Remap x from [a, b] to [s, t].
 * @param [in] a  Source min value.
 * @param [in] b  Source max value.
 * @param [in] s  Destination min value.
 * @param [in] t  Destination max value.
 * @param [in] x  Remap target value.
 * @return Remapped value.
 */
float remap(float a, float b, float s, float t, float x)
{
    return (t - s) * ((x - a) / (b - a)) + s;
}


/*!
 * @brief Horizontal multiply.
 * @param [in] v  A 2D-vector.
 * @return v.x * v.y
 */
float hmul(float2 v)
{
    return v.x * v.y;
}


/*!
 * @brief fmod() implementation in GLSL.
 * @param [in] x  First value.
 * @param [in] y  Second value.
 * @return mod value.
 */
float fmodglsl(float x, float y)
{
    return x - y * floor(x / y);
}


/*!
 * @brief fmod() implementation in GLSL.
 * @param [in] x  First value.
 * @param [in] y  Second value.
 * @return mod value.
 */
float3 fmodglsl(float3 x, float3 y)
{
    return x - y * floor(x / y);
}


/*!
 * @brief Calcurate a digit to show.
 * @param [in] val  First value.
 * @param [in] digitNum  Second value.
 * @return
 */
float calcDigit(float val, float digitNum)
{
    return floor(fmodglsl(val, digitNum) * 10 / digitNum);
}


/*!
 * @brief Get 2D-rotation matrix.
 *
 * @param [in] angle  Angle of rotation.
 * @return 2D-rotation matrix.
 */
float2x2 rotate2DMat(float angle)
{
    float s, c;
    sincos(angle, s, c);
    return float2x2(c, -s, s, c);
}


/*!
 * @brief Rotate on 2D plane
 *
 * @param [in] v  Target vector
 * @param [in] angle  Angle of rotation.
 * @return Rotated vector.
 */
float2 rotate2D(float2 v, float angle)
{
    return mul(rotate2DMat(angle), v);
}


/*!
 * @brief Inverse affine trasform UV coordinate.
 * @param [in] uv  Source UV.
 * @param [in] scale  Scaling factor.
 * @param [in] rotAngle  Rotation angle in degrees.
 * @param [in] translate Translate factor.
 * @return Affine transformed UV coordinate.
 */
float2 invAffineTransform(float2 uv, float2 scale, float rotAngle, float2 translate)
{
    static const float2 uvCenter = float2(0.5, 0.5);

    // translate -> rotate -> scale
    return rotate2D(uv - uvCenter + translate, radians(-rotAngle)) / scale + uvCenter;
}
