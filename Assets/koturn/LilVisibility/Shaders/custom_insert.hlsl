/*!
 * @brief Identify rendering in mirror or not.
 * @return True if rendering in mirror, otherwise false.
 */
bool isInMirror()
{
    return unity_CameraProjection[2][0] != 0.f || unity_CameraProjection[2][1] != 0.f;
}


#include "lil_opt_common_functions.hlsl"
#include "lil_override.hlsl"
