#if LIL_CURRENT_VERSION_MAJOR >= 2
static const uint _FurMeshType = 0;
#endif  // LIL_CURRENT_VERSION_MAJOR >= 2

#if defined(LIL_PASS_FORWARD_FUR_INCLUDED)
/*!
 * @brief Geometry shader function for fur shader.
 *
 * Original function: geom() in lil_common_vert_fur.hlsl.
 *
 * @param [in] input  Input triangle.
 * @param [in] outStream  Output triangle stream.
 * @param [in] primitiveID  Primitive ID.
 */
#if defined(LIL_ONEPASS_FUR)
[maxvertexcount(46)]
#elif (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)) && (defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP) || defined(LIL_LIGHTMODE_SHADOWMASK)) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP)
[maxvertexcount(32)]
#else
[maxvertexcount(40)]
#endif
void geomCustom(triangle v2g input[3], inout TriangleStream<v2f> outStream, uint primitiveID : SV_PrimitiveID)
{
    static const float3 baryCoords[3] = {
        float3(3.0, 0.0, 0.0),
        float3(0.0, 3.0, 0.0),
        float3(0.0, 0.0, 3.0)
    };
    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    [branch]
    if (_Invisible) {
        return;
    }

    // #if defined(LIL_ONEPASS_FUR)
        const float polyCrackStartTime = _PolygonRevivalTime + _PolygonRetentionTime;
        const float polyBurstStartTime = polyCrackStartTime + _PolygonCrackTime + _PolygonFillTime;
        const float oneCycleTime = polyBurstStartTime + _PolygonBurstTime + _PolygonDisappearTime;

        const float3 primitiveRand = iqint2(primitiveID.xxx) * (1.0 / float(0xffffffffU));
        const float cycleTime = fmodglsl(LIL_TIME, oneCycleTime);

        const float timeInBurst = primitiveRand.x * _PolygonBurstTime;
        if ((cycleTime - polyBurstStartTime) > timeInBurst) {
            return;
        }
    // #endif

    LIL_SETUP_INSTANCE_ID(input[0]);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);

    const float3 vertexRand = iqint2(asuint(float3(input[0].baryCoord.x, input[1].baryCoord.x, input[2].baryCoord.x))).xyz * (1.0 / float(0xffffffffU));
    const float3 emissionWeights = cycleTime - polyCrackStartTime > vertexRand * _PolygonCrackTime.xxx ? (1.0).xxx : (0.0).xxx;

    const float3 polyNormal = normalize(cross(input[1].positionWS - input[0].positionWS, input[2].positionWS - input[0].positionWS));

    if (cycleTime >= polyBurstStartTime) {
        const float3 centerPos = (input[0].positionWS + input[1].positionWS + input[2].positionWS) * (1.0 / 3.0);
        const float3 moveWS = polyNormal * (cycleTime - polyBurstStartTime) / _PolygonBurstTime * _PolygonMoveSpeed;
        for (uint i = 0; i < 3; i++) {
            input[i].positionWS = rotate3D(input[i].positionWS, centerPos, polyNormal, LIL_TIME * _PolygonRotSpeed * primitiveRand.y) + moveWS;
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    #if defined(LIL_ONEPASS_FUR)
        v2f outputBase[3];
        LIL_INITIALIZE_STRUCT(v2f, outputBase[0]);
        LIL_INITIALIZE_STRUCT(v2f, outputBase[1]);
        LIL_INITIALIZE_STRUCT(v2f, outputBase[2]);

        for (uint i = 0; i < 3; i++) {
            LIL_TRANSFER_INSTANCE_ID(input[i], outputBase[i]);
            LIL_TRANSFER_VERTEX_OUTPUT_STEREO(input[i], outputBase[i]);
            #if defined(LIL_V2F_POSITION_CS)
                outputBase[i].positionCS = lilTransformWStoCS(input[i].positionWS);
            #endif
            #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
                outputBase[i].previousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(input[i].previousPositionWS, 1.0));
            #endif
            #if defined(LIL_V2F_TEXCOORD0)
                outputBase[i].uv0 = input[i].uv0;
            #endif
            #if defined(LIL_V2F_NORMAL_WS)
                outputBase[i].normalWS = input[i].normalWS;
            #endif
            #if defined(LIL_V2F_FURLAYER)
                outputBase[i].furLayer = -2;
            #endif
            outputBase[i].baryCoord = baryCoords[i];
            outputBase[i].emissionWeights = emissionWeights;
        }

        // Front
        if (_Cull != 1) {
            outStream.Append(outputBase[0]);
            outStream.Append(outputBase[1]);
            outStream.Append(outputBase[2]);
            outStream.RestartStrip();
        }

        // Back
        if (_Cull != 2) {
            outStream.Append(outputBase[2]);
            outStream.Append(outputBase[1]);
            outStream.Append(outputBase[0]);
            outStream.RestartStrip();
        }
    #endif

    v2f output;
    LIL_INITIALIZE_STRUCT(v2f, output);
    LIL_TRANSFER_INSTANCE_ID(input[0], output);
    LIL_TRANSFER_VERTEX_OUTPUT_STEREO(input[0], output);

    //------------------------------------------------------------------------------------------------------------------------------
    // Main Light
    #if defined(LIL_V2G_LIGHTCOLOR)
        output.lightColor = input[0].lightColor;
    #endif
    #if defined(LIL_V2G_LIGHTDIRECTION)
        output.lightDirection = input[0].lightDirection;
    #endif
    #if defined(LIL_V2G_INDLIGHTCOLOR)
        output.indLightColor = input[0].indLightColor;
    #endif

    if (_FurMeshType) {
        #include "lil_common_vert_fur_thirdparty.hlsl"
    } else {
        float3 furVectors[3];
        furVectors[0] = input[0].furVector;
        furVectors[1] = input[1].furVector;
        furVectors[2] = input[2].furVector;
        #if !defined(LIL_NOT_SUPPORT_VERTEXID)
            uint3 n0 = (input[0].vertexID * 3 + input[1].vertexID * 1 + input[2].vertexID * 1) * uint3(1597334677U, 3812015801U, 2912667907U);
            uint3 n1 = (input[0].vertexID * 1 + input[1].vertexID * 3 + input[2].vertexID * 1) * uint3(1597334677U, 3812015801U, 2912667907U);
            uint3 n2 = (input[0].vertexID * 1 + input[1].vertexID * 1 + input[2].vertexID * 3) * uint3(1597334677U, 3812015801U, 2912667907U);
            float3 noise0 = normalize(float3(n0) * (2.0/float(0xffffffffU)) - 1.0);
            float3 noise1 = normalize(float3(n1) * (2.0/float(0xffffffffU)) - 1.0);
            float3 noise2 = normalize(float3(n2) * (2.0/float(0xffffffffU)) - 1.0);
            furVectors[0] += noise0 * (_FurVector.w * _FurRandomize);
            furVectors[1] += noise1 * (_FurVector.w * _FurRandomize);
            furVectors[2] += noise2 * (_FurVector.w * _FurRandomize);
        #endif
        #if defined(LIL_FEATURE_FurLengthMask)
            furVectors[0] *= LIL_SAMPLE_2D_LOD(_FurLengthMask, lil_sampler_linear_repeat, input[0].uv0 * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;
            furVectors[1] *= LIL_SAMPLE_2D_LOD(_FurLengthMask, lil_sampler_linear_repeat, input[1].uv0 * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;
            furVectors[2] *= LIL_SAMPLE_2D_LOD(_FurLengthMask, lil_sampler_linear_repeat, input[2].uv0 * _MainTex_ST.xy + _MainTex_ST.zw, 0).r;
        #endif

        static const float3 factors[12] = {
            float3(1.0, 0.0, 0.0) / 1.0,
            float3(0.0, 1.0, 0.0) / 1.0,
            float3(0.0, 0.0, 1.0) / 1.0,
            float3(0.0, 1.0, 1.0) / 2.0,
            float3(1.0, 0.0, 1.0) / 2.0,
            float3(1.0, 1.0, 0.0) / 2.0,
            float3(1.0, 4.0, 1.0) / 6.0,
            float3(0.0, 1.0, 1.0) / 2.0,
            float3(1.0, 1.0, 4.0) / 6.0,
            float3(1.0, 0.0, 1.0) / 2.0,
            float3(4.0, 1.0, 1.0) / 6.0,
            float3(1.0, 1.0, 0.0) / 2.0
        };

        int loopEnd = _FurLayerNum >= 3 ? 12 : _FurLayerNum >= 2 ? 6 : 3;
        [loop]
        for (int i = 0; i < loopEnd; i++) {
            AppendFur(outStream, output, input, furVectors, factors[i]);
        }
        AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
        outStream.RestartStrip();
    }
}
#elif defined(LIL_ONEPASS_OUTLINE)
/*!
 * @brief Geometry shader function for outline shader.
 *
 * Original function: geom() in lil_common_vert.hlsl.
 *
 * @param [in] input  Input triangle.
 * @param [in] outStream  Output triangle stream.
 * @param [in] primitiveID  Primitive ID.
 */
[maxvertexcount(12)]
void geomCustom(triangle v2f input[3], inout TriangleStream<v2f> outStream, uint primitiveID : SV_PrimitiveID)
{
    static const float3 baryCoords[3] = {
        float3(3.0, 0.0, 0.0),
        float3(0.0, 3.0, 0.0),
        float3(0.0, 0.0, 3.0)
    };
    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    [branch]
    if (_Invisible) {
        return;
    }

    const float polyCrackStartTime = _PolygonRevivalTime + _PolygonRetentionTime;
    const float polyBurstStartTime = polyCrackStartTime + _PolygonCrackTime + _PolygonFillTime;
    const float oneCycleTime = polyBurstStartTime + _PolygonBurstTime + _PolygonDisappearTime;

    const float3 primitiveRand = iqint2(primitiveID.xxx) * (1.0 / float(0xffffffffU));
    const float cycleTime = fmodglsl(LIL_TIME, oneCycleTime);

    const float timeInBurst = primitiveRand.x * _PolygonBurstTime;
    if ((cycleTime - polyBurstStartTime) > timeInBurst) {
        return;
    }

    v2f output[3];
    LIL_INITIALIZE_STRUCT(v2f, output[0]);
    LIL_INITIALIZE_STRUCT(v2f, output[1]);
    LIL_INITIALIZE_STRUCT(v2f, output[2]);

    LIL_SETUP_INSTANCE_ID(input[0].base);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0].base);

    const float3 vertexRand = iqint2(asuint(float3(input[0].baryCoord.x, input[1].baryCoord.x, input[2].baryCoord.x))).xyz * (1.0 / float(0xffffffffU));
    const float3 emissionWeights = cycleTime - polyCrackStartTime > vertexRand * _PolygonCrackTime.xxx ? (1.0).xxx : (0.0).xxx;

    const float3 polyNormal = normalize(cross(input[1].positionWS - input[0].positionWS, input[2].positionWS - input[0].positionWS));

    if (cycleTime >= polyBurstStartTime) {
        const float3 centerPos = (input[0].positionWS + input[1].positionWS + input[2].positionWS) * (1.0 / 3.0);
        const float3 moveWS = polyNormal * (cycleTime - polyBurstStartTime) / _PolygonBurstTime * _PolygonMoveSpeed;
        for (uint i = 0; i < 3; i++) {
            input[i].positionWS = rotate3D(input[i].positionWS, centerPos, polyNormal, LIL_TIME * _PolygonRotSpeed * primitiveRand.y) + moveWS;
            input[i].positionCS = lilTransformWStoCS(input[i].positionWS);
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    [unroll]
    for (uint i = 0; i < 3; i++) {
        output[i] = input[i].base;
        output[i].baryCoord = baryCoords[i];
        output[i].emissionWeights = emissionWeights;
    }

    // Front
    if (_Cull != 1) {
        outStream.Append(output[0]);
        outStream.Append(output[1]);
        outStream.Append(output[2]);
        outStream.RestartStrip();
    }

    // Back
    if (_Cull != 2) {
        outStream.Append(output[2]);
        outStream.Append(output[1]);
        outStream.Append(output[0]);
        outStream.RestartStrip();
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Outline
    [unroll]
    for (uint j = 0; j < 3; j++) {
        output[j].positionCS = input[j].positionCSOL;
        #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
            output[j].previousPositionCS = input[j].previousPositionCSOL;
        #endif
    }

    // Front
    if (_OutlineCull != 1) {
        outStream.Append(output[0]);
        outStream.Append(output[1]);
        outStream.Append(output[2]);
        outStream.RestartStrip();
    }

    // Back
    if (_OutlineCull != 2) {
        outStream.Append(output[2]);
        outStream.Append(output[1]);
        outStream.Append(output[0]);
        outStream.RestartStrip();
    }
}
#else
/*!
 * @brief Geometry shader function for outline shader.
 * @param [in] input  Input triangle.
 * @param [in] outStream  Output triangle stream.
 * @param [in] primitiveID  Primitive ID.
 */
[maxvertexcount(3)]
void geomCustom(triangle v2f input[3], inout TriangleStream<v2f> outStream, uint primitiveID : SV_PrimitiveID)
{
    static const float3 baryCoords[3] = {
        float3(3.0, 0.0, 0.0),
        float3(0.0, 3.0, 0.0),
        float3(0.0, 0.0, 3.0)
    };
    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    [branch]
    if (_Invisible) {
        return;
    }

    const float polyCrackStartTime = _PolygonRevivalTime + _PolygonRetentionTime;
    const float polyBurstStartTime = polyCrackStartTime + _PolygonCrackTime + _PolygonFillTime;
    const float oneCycleTime = polyBurstStartTime + _PolygonBurstTime + _PolygonDisappearTime;

    const float3 primitiveRand = iqint2(primitiveID.xxx) * (1.0 / float(0xffffffffU));
    const float cycleTime = fmodglsl(LIL_TIME, oneCycleTime);

    const float timeInBurst = primitiveRand.x * _PolygonBurstTime;
    if ((cycleTime - polyBurstStartTime) > timeInBurst) {
        return;
    }

    v2f output[3];
    LIL_INITIALIZE_STRUCT(v2f, output[0]);
    LIL_INITIALIZE_STRUCT(v2f, output[1]);
    LIL_INITIALIZE_STRUCT(v2f, output[2]);

    LIL_SETUP_INSTANCE_ID(input[0]);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);

    const float3 vertexRand = iqint2(asuint(float3(input[0].baryCoord.x, input[1].baryCoord.x, input[2].baryCoord.x))).xyz * (1.0 / float(0xffffffffU));
    const float3 emissionWeights = cycleTime - polyCrackStartTime > vertexRand * _PolygonCrackTime.xxx ? (1.0).xxx : (0.0).xxx;

    const float3 polyNormal = normalize(cross(input[1].positionWS - input[0].positionWS, input[2].positionWS - input[0].positionWS));

    if (cycleTime >= polyBurstStartTime) {
        const float3 centerPos = (input[0].positionWS + input[1].positionWS + input[2].positionWS) * (1.0 / 3.0);
        const float3 moveWS = polyNormal * (cycleTime - polyBurstStartTime) / _PolygonBurstTime * _PolygonMoveSpeed;
        [unroll]
        for (uint i = 0; i < 3; i++) {
            input[i].positionWS = rotate3D(input[i].positionWS, centerPos, polyNormal, LIL_TIME * _PolygonRotSpeed * primitiveRand.y) + moveWS;
            input[i].positionCS = lilTransformWStoCS(input[i].positionWS);
        }
    }

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    [unroll]
    for (uint i = 0; i < 3; i++) {
        input[i].baryCoord = baryCoords[i];
        input[i].emissionWeights = emissionWeights;
        outStream.Append(input[i]);
    }

    outStream.RestartStrip();
}
#endif  // defined(LIL_PASS_FORWARD_FUR_INCLUDED)


#include "lil_opt_vert.hlsl"
