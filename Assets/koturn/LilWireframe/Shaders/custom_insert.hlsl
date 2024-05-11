#if defined(USE_CUSTOM_GEOMETRY)
#    if LIL_CURRENT_VERSION_VALUE == 34 && defined(UNITY_PASS_SHADOWCASTER)
// Work around for the following bug in lilxyzw/lilToon ver.1.4.0:
//   https://github.com/lilxyzw/lilToon/issues/98
// Fixed in lilxyzw/lilToon ver.1.4.1:
//   https://github.com/lilxyzw/lilToon/commit/a8548792c56537575bb2933d65233c8c9bdca4de
#        define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
            float3 baryCoord : TEXCOORD ## id1; \
            nointerpolation float3 color0 : TEXCOORD ## id2; \
            nointerpolation float3 color1 : TEXCOORD ## id3; \
            nointerpolation float3 color2 : TEXCOORD ## id4; \
            nointerpolation float3 emissionWeights : TEXCOORD ## id5;
#    else
#        define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3,id4,id5,id6,id7) \
            float3 baryCoord : TEXCOORD ## id0; \
            nointerpolation float3 color0 : TEXCOORD ## id1; \
            nointerpolation float3 color1 : TEXCOORD ## id2; \
            nointerpolation float3 color2 : TEXCOORD ## id3; \
            nointerpolation float3 emissionWeights : TEXCOORD ## id4;
#    endif  // LIL_CURRENT_VERSION_VALUE == 34 && defined(UNITY_PASS_SHADOWCASTER)

// Add vertex copy
#    define LIL_CUSTOM_VERT_COPY \
        LIL_V2F_OUT.baryCoord.x = (float)input.vertexID;

// Inserting a process into the vertex shader
//#define LIL_CUSTOM_VERTEX_OS
//#define LIL_CUSTOM_VERTEX_WS

// Inserting a process into pixel shader
//#define BEFORE_xx
//#define OVERRIDE_xx
#    define BEFORE_BLEND_EMISSION \
        const float3 emissionWeights = (input.baryCoord < _WireframeWidth) ? input.emissionWeights : (0.0).xxx; \
        const float emissionWeight = max(emissionWeights.x, max(emissionWeights.y, emissionWeights.z)); \
        const float3 emissionColor = emissionWeight == emissionWeights.x ? input.color0 \
            : emissionWeight == emissionWeights.y ? input.color1 \
            : input.color2; \
        fd.col.rgb += calcEmissionColor(LIL_SAMPLE_2D(_WireframeMask, sampler_MainTex, fd.uvMain) * emissionColor * emissionWeight, fd.col.a);
#endif  // defined(USE_CUSTOM_GEOMETRY)


/*!
 * @brief Calculate emission color.
 * @param [in] emissionColor  Base emission color.
 * @param [in] alpha  Alpha value.
 * @return Calculated emission color.
 */
float3 calcEmissionColor(float3 emissionColor, float alpha)
{
#if LIL_RENDER == 2 && !defined(LIL_REFRACTION)
    return emissionColor * alpha;
#else
    return emissionColor;
#endif  // LIL_RENDER == 2 && !defined(LIL_REFRACTION)
}


#include "lil_opt_common_functions.hlsl"
#include "lil_override.hlsl"
