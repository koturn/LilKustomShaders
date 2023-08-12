#include "LilOptCommonFunctions.hlsl"
#include "LilOverride.hlsl"


/*!
 * @brief Obtain reflection direction considering box projection.
 *
 * This function is more efficient than BoxProjectedCubemapDirection() in UnityStandardUtils.cginc.
 *
 * @param [in] refDir  Refrection dir (must be normalized).
 * @param [in] worldPos  World coordinate.
 * @param [in] probePos  Position of Refrection probe.
 * @param [in] boxMin  Position of Refrection probe.
 * @param [in] boxMax  Position of Refrection probe.
 * @return Refrection direction considering box projection.
 */
float3 boxProj(float3 refDir, float3 worldPos, float4 probePos, float4 boxMin, float4 boxMax)
{
    // UNITY_SPECCUBE_BOX_PROJECTION is defined if
    // "Reflection Probes Box Projection" of GraphicsSettings is enabled.
#ifdef UNITY_SPECCUBE_BOX_PROJECTION
    // probePos.w == 1.0 if Box Projection is enabled.
    if (probePos.w > 0.0) {
        const float3 magnitudes = ((refDir > 0.0 ? boxMax.xyz : boxMin.xyz) - worldPos) / refDir;
        refDir = refDir * min(magnitudes.x, min(magnitudes.y, magnitudes.z)) + (worldPos - probePos);
    }
#endif  // UNITY_SPECCUBE_BOX_PROJECTION

    return refDir;
}


/*!
 * @brief Get reflection direction of the first reflection probe
 * considering box projection.
 *
 * @param [in] refDir  Refrection dir (must be normalized).
 * @param [in] worldPos  World coordinate.
 * @return Refrection direction considering box projection.
 */
float3 boxProj0(float3 refDir, float3 worldPos)
{
    return boxProj(
        refDir,
        worldPos,
        unity_SpecCube0_ProbePosition,
        unity_SpecCube0_BoxMin,
        unity_SpecCube0_BoxMax);
}


/*!
 * @brief Get reflection direction of the second reflection probe
 * considering box projection.
 *
 * @param [in] refDir  Refrection dir (must be normalized).
 * @param [in] worldPos  World coordinate.
 * @return Refrection direction considering box projection.
 */
float3 boxProj1(float3 refDir, float3 worldPos)
{
    return boxProj(
        refDir,
        worldPos,
        unity_SpecCube1_ProbePosition,
        unity_SpecCube1_BoxMin,
        unity_SpecCube1_BoxMax);
}


/*!
 * @brief Get color of the first reflection probe.
 *
 * @param [in] refDir  Reflect direction (must be normalized).
 * @param [in] worldPos  World coordinate.
 * @return Color of the first reflection probe.
 */
half4 getRefProbeColor0(float3 refDir, float3 worldPos)
{
    half4 refColor = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, boxProj0(refDir, worldPos), 0.0);
    refColor.rgb = DecodeHDR(refColor, unity_SpecCube0_HDR);
    return refColor;
}


/*!
 * @brief Get color of the second reflection probe.
 *
 * @param [in] refDir  Reflect direction (must be normalized).
 * @param [in] worldPos  World coordinate.
 * @return Color of the second reflection probe.
 */
half4 getRefProbeColor1(float3 refDir, float3 worldPos)
{
    half4 refColor = UNITY_SAMPLE_TEXCUBE_SAMPLER_LOD(unity_SpecCube1, unity_SpecCube0, boxProj1(refDir, worldPos), 0.0);
    refColor.rgb = DecodeHDR(refColor, unity_SpecCube1_HDR);
    return refColor;
}


/*!
 * @brief Get blended color of the two reflection probes.
 *
 * @param [in] refDir  Reflect direction (must be normalized).
 * @param [in] worldPos  World coordinate.
 * @return Color of reflection probe.
 */
half4 getRefProbeColor(float3 refDir, float3 worldPos)
{
    return lerp(
        getRefProbeColor1(refDir, worldPos),
        getRefProbeColor0(refDir, worldPos),
        unity_SpecCube0_BoxMin.w);
}
