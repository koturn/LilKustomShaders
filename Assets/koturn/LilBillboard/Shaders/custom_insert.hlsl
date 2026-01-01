static const int BILLBOARD_MODE_OFF = 0;
static const int BILLBOARD_MODE_FULL = 1;
static const int BILLBOARD_MODE_YAXIS = 2;


float3 applyBillboardEuler(float3 v, float3 eulerDeg)
{
    float3 s;
    float3 c;
    sincos(radians(eulerDeg), /* out */ s, /* out */ c);

    v = float3(v.x, v.y * c.x - v.z * s.x, v.y * s.x + v.z * c.x);
    v = float3(v.x * c.y + v.z * s.y, v.y, -v.x * s.y + v.z * c.y);
    v = float3(v.x * c.z - v.y * s.z, v.x * s.z + v.y * c.z, v.z);
    return v;
}


// localPosOS : メッシュのローカル頂点 (positionOS)
// posWS      : 既に計算済みのワールド座標（書き換える）
// normalWS   : 同上
// tangentWS, bitangentWS : 同上
void LilBillboardVertexWS(float3 positionOS, inout float3 positionWS, inout float3 normalWS, inout float3 tangentWS, inout float3 bitangentWS)
{
    if (_BillboardMode == BILLBOARD_MODE_OFF) {
        return;
    }

    const float3 centerWS = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0)).xyz;
#if defined(UNITY_PASS_SHADOWCASTER)
    float3 toCamera;
    if (LIL_MATRIX_P[3][3] == 1.0) {
        toCamera = LIL_MATRIX_V[2].xyz;
    } else if (abs(unity_LightShadowBias.x) < 1.0e-5) {
        toCamera = _WorldSpaceCameraPos.xyz - centerWS;
    } else {
        // toCamera = -getCameraDirVec(screenPos);
        toCamera = LIL_MATRIX_V[2].xyz;
    }
#else
    const float3 toCamera = normalize(_WorldSpaceCameraPos - centerWS);
#endif  // defined(UNITY_PASS_SHADOWCASTER)

    float3 rightWS;
    float3 upWS;
    float3 forwardWS;

    if (_BillboardMode == BILLBOARD_MODE_YAXIS) {
        upWS = float3(0.0, 1.0, 0.0);
        float3 toCamXZ = float3(toCamera.x, 0.0, toCamera.z);
        if (all(abs(toCamXZ) < 1.0e-5)) {
            toCamXZ = float3(0.0, 0.0, 1.0);
        }
        toCamXZ = normalize(toCamXZ);
        forwardWS = toCamXZ;
        rightWS = normalize(cross(upWS, forwardWS));
        forwardWS = normalize(cross(rightWS, upWS));
    } else {
        forwardWS = normalize(toCamera);

        upWS = float3(0.0, 1.0, 0.0);
        if (abs(dot(forwardWS, upWS)) > 0.99) {
            upWS = float3(0.0, 0.0, 1.0);
        }

        rightWS = normalize(cross(upWS, forwardWS));
        upWS = normalize(cross(forwardWS, rightWS));
    }

    float3 localOffset = positionOS * _BillboardScale.xyz;

    float3 euler = _BillboardRotation.xyz;
    if (any(euler != 0.0)) {
        localOffset = applyBillboardEuler(localOffset, euler);
    }

    positionWS = centerWS + rightWS * localOffset.x + upWS * localOffset.y + forwardWS * localOffset.z;

    normalWS = forwardWS;
    tangentWS = rightWS;
    bitangentWS = upWS;
}



// float4 objectToClipPos(float3 positionOS, inout float3 positionWS, inout float3 normalWS, inout float3 tangentWS, inout float3 bitangentWS)
// {
//     if (_BillboardMode == BILLBOARD_MODE_OFF) {
//         return;
//     }
//
//     if (_BillboardMode == BILLBOARD_MODE_YAXIS) {
//         const float3 srWorldPos = mul((float3x3)LIL_MATRIX_M, positionOS);
//         const float4 viewPos = mul(LIL_MATRIX_V, LIL_MATRIX_M._m03_m13_m23_m33) + float4(
//             dot(float3(1.0, LIL_MATRIX_V._m01, 0.0), srWorldPos),
//             dot(float3(0.0, LIL_MATRIX_V._m11, 0.0), srWorldPos),
//             dot(float3(0.0, LIL_MATRIX_V._m21, -1.0), srWorldPos),
//             0.0);
//         return mul(LIL_MATRIX_P, viewPos);
//     }
//     else
//     {
//         float4 viewOffset = float4(mul((float3x3)LIL_MATRIX_M, positionOS), 0.0);
//         viewOffset.z = -viewOffset.z;
//         const float4 viewPos = mul(LIL_MATRIX_V, LIL_MATRIX_M._m03_m13_m23_m33) + viewOffset;
//         return mul(LIL_MATRIX_P, viewPos);
//     }
// }
