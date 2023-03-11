#ifndef LIL_OPT_VERT_INCLUDED
#define LIL_OPT_VERT_INCLUDED


#if defined(LIL_PASS_FORWARD_FUR_INCLUDED)
v2g vertOpt(appdata input)
{
    v2g output;
    LIL_INITIALIZE_STRUCT(v2g, output);

    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    if(_Invisible) return output;

    //------------------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, output);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv0, _MainTex_ST);
    float2 uvs[4] = {uvMain,input.uv1,input.uv2,input.uv3};

    //------------------------------------------------------------------------------------------------------------------------------
    // Vertex Modification
    #include "lil_vert_encryption.hlsl"
    lilCustomVertexOS(input, uvMain, input.positionOS);

    //------------------------------------------------------------------------------------------------------------------------------
    // Previous Position (for HDRP)
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        input.previousPositionOS = unity_MotionVectorsParams.x > 0.0 ? input.previousPositionOS : input.positionOS.xyz;
        #if defined(_ADD_PRECOMPUTED_VELOCITY)
            input.previousPositionOS -= input.precomputedVelocity;
        #endif
        #define LIL_MODIFY_PREVPOS
        #include "lil_vert_encryption.hlsl"
        lilCustomVertexOS(input, uvMain, input.previousPositionOS);
        #undef LIL_MODIFY_PREVPOS

        //------------------------------------------------------------------------------------------------------------------------------
        // Transform
        LIL_VERTEX_POSITION_INPUTS(input.previousPositionOS, previousVertexInput);
        #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
            LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, previousVertexNormalInput);
        #elif defined(LIL_APP_NORMAL)
            LIL_VERTEX_NORMAL_INPUTS(input.normalOS, previousVertexNormalInput);
        #else
            lilVertexNormalInputs previousVertexNormalInput = lilGetVertexNormalInputs();
        #endif
        previousVertexInput.positionWS = TransformPreviousObjectToWorld(input.previousPositionOS);
        lilCustomVertexWS(input, uvMain, previousVertexInput, previousVertexNormalInput);
        output.previousPositionWS = previousVertexInput.positionWS;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Transform
    LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
    lilCustomVertexWS(input, uvMain, vertexInput, vertexNormalInput);
    #if defined(LIL_CUSTOM_VERTEX_WS)
        LIL_RE_VERTEX_POSITION_INPUTS(vertexInput);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    #if defined(LIL_V2G_POSITION_WS)
        output.positionWS       = vertexInput.positionWS;
    #endif
    #if defined(LIL_V2G_TEXCOORD0)
        output.uv0              = input.uv0;
    #endif
    #if defined(LIL_V2G_TEXCOORD1)
        output.uv1              = input.uv1;
    #endif
    #if defined(LIL_V2G_TEXCOORD2)
        output.uv2              = input.uv2;
    #endif
    #if defined(LIL_V2G_TEXCOORD3)
        output.uv3              = input.uv3;
    #endif
    #if defined(LIL_V2G_PACKED_TEXCOORD01)
        output.uv01.xy          = input.uv0;
        output.uv01.zw          = input.uv1;
    #endif
    #if defined(LIL_V2G_PACKED_TEXCOORD23)
        output.uv23.xy          = input.uv2;
        output.uv23.zw          = input.uv3;
    #endif
    #if !defined(LIL_NOT_SUPPORT_VERTEXID) && defined(LIL_V2G_VERTEXID)
        output.vertexID         = input.vertexID;
    #endif
    #if defined(LIL_V2G_NORMAL_WS)
        output.normalWS         = vertexNormalInput.normalWS;
    #endif

    LIL_CUSTOM_VERT_COPY

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog & Lighting
    lilFragData fd = lilInitFragData();
    LIL_GET_HDRPDATA(vertexInput,fd);
    #if defined(LIL_V2F_LIGHTCOLOR) || defined(LIL_V2F_LIGHTDIRECTION) || defined(LIL_V2F_INDLIGHTCOLOR)
        LIL_CALC_MAINLIGHT(vertexInput, lightdataInput);
    #endif
    #if defined(LIL_V2F_LIGHTCOLOR)
        output.lightColor       = lightdataInput.lightColor;
    #endif
    #if defined(LIL_V2F_LIGHTDIRECTION)
        output.lightDirection   = lightdataInput.lightDirection;
    #endif
    #if defined(LIL_V2F_INDLIGHTCOLOR)
        output.indLightColor    = lightdataInput.indLightColor * _ShadowEnvStrength;
    #endif
    #if defined(LIL_V2G_SHADOW)
        LIL_TRANSFER_SHADOW(vertexInput, input.uv1, output);
    #endif
    #if defined(LIL_V2G_VERTEXLIGHT_FOG)
        LIL_TRANSFER_FOG(vertexInput, output);
        LIL_CALC_VERTEXLIGHT(vertexInput, output);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Vector
    #if defined(LIL_V2G_FURVECTOR)
        float3 bitangentOS = normalize(cross(input.normalOS, input.tangentOS.xyz)) * (input.tangentOS.w * length(input.normalOS));
        float3x3 tbnOS = float3x3(input.tangentOS.xyz, bitangentOS, input.normalOS);
        output.furVector = _FurVector.xyz + float3(0,0,0.001);
        if(_VertexColor2FurVector) output.furVector = lilBlendNormal(output.furVector, input.color.xyz);
        #if defined(LIL_FEATURE_FurVectorTex)
            output.furVector = lilBlendNormal(output.furVector, lilUnpackNormalScale(LIL_SAMPLE_2D_LOD(_FurVectorTex, lil_sampler_linear_repeat, uvMain, 0), _FurVectorScale));
        #endif
        output.furVector = mul(normalize(output.furVector), tbnOS);
        output.furVector *= _FurVector.w;
        #if defined(LIL_FUR_PRE)
            output.furVector *= _FurCutoutLength;
        #endif
        output.furVector = lilTransformDirOStoWS(output.furVector, false);
        float furLength = length(output.furVector);
        output.furVector.y -= _FurGravity * furLength;

        #if defined(LIL_FEATURE_FUR_COLLISION) && defined(LIL_BRP) && defined(VERTEXLIGHT_ON)
            // Touch
            float3 positionWS2 = output.positionWS + output.furVector;
            float4 toLightX = unity_4LightPosX0 - positionWS2.x;
            float4 toLightY = unity_4LightPosY0 - positionWS2.y;
            float4 toLightZ = unity_4LightPosZ0 - positionWS2.z;
            float4 lengthSq = toLightX * toLightX + 0.000001;
            lengthSq += toLightY * toLightY;
            lengthSq += toLightZ * toLightZ;
            float4 atten = saturate(1.0 - lengthSq * unity_4LightAtten0 / 25.0) * _FurTouchStrength * furLength;
            //float4 rangeToggle = abs(frac(sqrt(25.0 / unity_4LightAtten0) * 100.0) - 0.22);
            float4 rangeToggle = abs(frac(sqrt(250000 / unity_4LightAtten0)) - 0.22);
            bool4 selector = rangeToggle < (0.001).xxxx - float4(
                dot(unity_LightColor[0].rgb, float3(1.0, 1.0, 1.0)),
                dot(unity_LightColor[1].rgb, float3(1.0, 1.0, 1.0)),
                dot(unity_LightColor[2].rgb, float3(1.0, 1.0, 1.0)),
                dot(unity_LightColor[3].rgb, float3(1.0, 1.0, 1.0)));
            float4 lenAtten = rsqrt(lengthSq) * atten;
            output.furVector = selector[0] ? output.furVector - float3(toLightX[0], toLightY[0], toLightZ[0]) * lenAtten[0] : output.furVector;
            output.furVector = selector[1] ? output.furVector - float3(toLightX[1], toLightY[1], toLightZ[1]) * lenAtten[1] : output.furVector;
            output.furVector = selector[2] ? output.furVector - float3(toLightX[2], toLightY[2], toLightZ[2]) * lenAtten[2] : output.furVector;
            output.furVector = selector[3] ? output.furVector - float3(toLightX[3], toLightY[3], toLightZ[3]) * lenAtten[3] : output.furVector;
        #endif
    #endif

    return output;
}


#if defined(LIL_ONEPASS_FUR)
    [maxvertexcount(46)]
#elif (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)) && (defined(LIL_USE_LIGHTMAP) || defined(LIL_USE_DYNAMICLIGHTMAP) || defined(LIL_LIGHTMODE_SHADOWMASK)) || defined(LIL_FEATURE_DISTANCE_FADE) || !defined(LIL_BRP)
    [maxvertexcount(32)]
#else
    [maxvertexcount(40)]
#endif
void geomOpt(triangle v2g input[3], inout TriangleStream<v2f> outStream)
{
    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    if(_Invisible) return;

    LIL_SETUP_INSTANCE_ID(input[0]);
    LIL_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input[0]);

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy
    #if defined(LIL_ONEPASS_FUR)
        v2f outputBase[3];
        LIL_INITIALIZE_STRUCT(v2f, outputBase[0]);
        LIL_INITIALIZE_STRUCT(v2f, outputBase[1]);
        LIL_INITIALIZE_STRUCT(v2f, outputBase[2]);

        for(uint i = 0; i < 3; i++)
        {
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
        }

        // Front
        if(_Cull != 1)
        {
            outStream.Append(outputBase[0]);
            outStream.Append(outputBase[1]);
            outStream.Append(outputBase[2]);
            outStream.RestartStrip();
        }

        // Back
        if(_Cull != 2)
        {
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

    if(_FurMeshType)
    {
        #include "lil_common_vert_fur_thirdparty.hlsl"
    }
    else
    {
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

#if 0
        if(_FurLayerNum == 1)
        {
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 0.0, 1.0) / 1.0);
        }
        else if(_FurLayerNum >= 2)
        {
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 0.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 0.0, 1.0) / 1.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 0.0) / 2.0);
        }
        if(_FurLayerNum >= 3)
        {
            AppendFur(outStream, output, input, furVectors, float3(1.0, 4.0, 1.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 4.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(4.0, 1.0, 1.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 0.0) / 2.0);
        }
#elif 0
        AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
        AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 0.0) / 1.0);
        AppendFur(outStream, output, input, furVectors, float3(0.0, 0.0, 1.0) / 1.0);
        if(_FurLayerNum >= 2)
        {
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 0.0) / 2.0);
        }
        if(_FurLayerNum >= 3)
        {
            AppendFur(outStream, output, input, furVectors, float3(1.0, 4.0, 1.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(0.0, 1.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 4.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 1.0) / 2.0);
            AppendFur(outStream, output, input, furVectors, float3(4.0, 1.0, 1.0) / 6.0);
            AppendFur(outStream, output, input, furVectors, float3(1.0, 1.0, 0.0) / 2.0);
        }
#else
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
        UNITY_LOOP
        for (int i = 0; i < loopEnd; i++) {
            AppendFur(outStream, output, input, furVectors, factors[i]);
        }
#endif
        AppendFur(outStream, output, input, furVectors, float3(1.0, 0.0, 0.0) / 1.0);
        outStream.RestartStrip();
    }
}


#else


#undef LIL_V2F_OUT_BASE
#undef LIL_V2F_OUT
#undef LIL_V2F_TYPE
#if defined(LIL_ONEPASS_OUTLINE)
    #define LIL_V2F_OUT_BASE output.base
    #define LIL_V2F_OUT output
    #define LIL_V2F_TYPE v2g
#else
    #define LIL_V2F_OUT_BASE output
    #define LIL_V2F_OUT output
    #define LIL_V2F_TYPE v2f
#endif


LIL_V2F_TYPE vertOpt(appdata input)
{
    LIL_V2F_TYPE LIL_V2F_OUT;
    LIL_INITIALIZE_STRUCT(v2f, LIL_V2F_OUT_BASE);
    LIL_INITIALIZE_STRUCT(LIL_V2F_TYPE, LIL_V2F_OUT);
    #if defined(LIL_ONEPASS_OUTLINE)
        LIL_V2F_OUT.positionCSOL = 0.0;
        #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
            LIL_V2F_OUT.previousPositionCSOL = 0.0;
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Invisible
    #if defined(LIL_OUTLINE) && !defined(LIL_LITE) && defined(USING_STEREO_MATRICES)
        #define LIL_VERTEX_CONDITION (_Invisible || _OutlineDisableInVR)
    #elif defined(LIL_OUTLINE) && !defined(LIL_LITE)
        #define LIL_VERTEX_CONDITION (_Invisible || _OutlineDisableInVR && (abs(UNITY_MATRIX_P._m02) > 0.000001))
    #else
        #define LIL_VERTEX_CONDITION (_Invisible)
    #endif

    #if defined(LIL_TESSELLATION) || defined(LIL_CUSTOM_SAFEVERT)
        if(!LIL_VERTEX_CONDITION)
        {
    #else
        if(LIL_VERTEX_CONDITION) return LIL_V2F_OUT;
    #endif

    #undef LIL_VERTEX_CONDITION

    //------------------------------------------------------------------------------------------------------------------------------
    // Single Pass Instanced rendering
    LIL_SETUP_INSTANCE_ID(input);
    LIL_TRANSFER_INSTANCE_ID(input, LIL_V2F_OUT_BASE);
    LIL_INITIALIZE_VERTEX_OUTPUT_STEREO(LIL_V2F_OUT_BASE);

    //------------------------------------------------------------------------------------------------------------------------------
    // UV
    float2 uvMain = lilCalcUV(input.uv0, _MainTex_ST);
    float2 uvs[4] = {uvMain,input.uv1,input.uv2,input.uv3};

    //------------------------------------------------------------------------------------------------------------------------------
    // Object space direction
    #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
        float3 bitangentOS = normalize(cross(input.normalOS, input.tangentOS.xyz)) * (input.tangentOS.w * length(input.normalOS));
        float3x3 tbnOS = float3x3(input.tangentOS.xyz, bitangentOS, input.normalOS);
    #else
        float3 bitangentOS = 0.0;
        float3x3 tbnOS = 0.0;
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Vertex Modification
    #include "lil_vert_encryption.hlsl"
    lilCustomVertexOS(input, uvMain, input.positionOS);
    #include "lil_vert_audiolink.hlsl"
    #if !defined(LIL_ONEPASS_OUTLINE)
        #include "lil_vert_outline.hlsl"
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Previous Position (for HDRP)
    #if defined(LIL_PASS_MOTIONVECTOR_INCLUDED)
        input.previousPositionOS = unity_MotionVectorsParams.x > 0.0 ? input.previousPositionOS : input.positionOS.xyz;
        #if defined(_ADD_PRECOMPUTED_VELOCITY)
            input.previousPositionOS -= input.precomputedVelocity;
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Vertex Modification
        #define LIL_MODIFY_PREVPOS
        #include "lil_vert_encryption.hlsl"
        lilCustomVertexOS(input, uvMain, input.previousPositionOS);
        #include "lil_vert_audiolink.hlsl"
        #undef LIL_MODIFY_PREVPOS

        //------------------------------------------------------------------------------------------------------------------------------
        // Transform
        LIL_VERTEX_POSITION_INPUTS(input.previousPositionOS, previousVertexInput);
        #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
            LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, previousVertexNormalInput);
        #elif defined(LIL_APP_NORMAL)
            LIL_VERTEX_NORMAL_INPUTS(input.normalOS, previousVertexNormalInput);
        #else
            lilVertexNormalInputs previousVertexNormalInput = lilGetVertexNormalInputs();
        #endif
        previousVertexInput.positionWS = TransformPreviousObjectToWorld(input.previousPositionOS);
        lilCustomVertexWS(input, uvMain, previousVertexInput, previousVertexNormalInput);
        LIL_V2F_OUT_BASE.previousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousVertexInput.positionWS, 1.0));

        #if defined(LIL_ONEPASS_OUTLINE)
            #define LIL_MODIFY_PREVPOS
            #include "lil_vert_outline.hlsl"
            #undef LIL_MODIFY_PREVPOS
            LIL_VERTEX_POSITION_INPUTS(input.previousPositionOS, previousOLVertexInput);
            previousOLVertexInput.positionWS = TransformPreviousObjectToWorld(input.previousPositionOS);
            lilCustomVertexWS(input, uvMain, previousOLVertexInput, previousVertexNormalInput);
            LIL_V2F_OUT.previousPositionCSOL = mul(UNITY_MATRIX_PREV_VP, float4(previousOLVertexInput.positionWS, 1.0));
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Transform
    #if defined(LIL_APP_POSITION)
        LIL_VERTEX_POSITION_INPUTS(input.positionOS, vertexInput);
    #endif
    #if defined(LIL_APP_NORMAL) && defined(LIL_APP_TANGENT)
        LIL_VERTEX_NORMAL_TANGENT_INPUTS(input.normalOS, input.tangentOS, vertexNormalInput);
    #elif defined(LIL_APP_NORMAL)
        LIL_VERTEX_NORMAL_INPUTS(input.normalOS, vertexNormalInput);
    #else
        lilVertexNormalInputs vertexNormalInput = lilGetVertexNormalInputs();
    #endif
    lilCustomVertexWS(input, uvMain, vertexInput, vertexNormalInput);
    #if defined(LIL_CUSTOM_VERTEX_WS)
        LIL_RE_VERTEX_POSITION_INPUTS(vertexInput);
    #endif
    float3 viewDirection = normalize(lilViewDirection(lilToAbsolutePositionWS(vertexInput.positionWS)));
    float3 headDirection = normalize(lilHeadDirection(lilToAbsolutePositionWS(vertexInput.positionWS)));

    //------------------------------------------------------------------------------------------------------------------------------
    // Copy

    // UV
    #if defined(LIL_V2F_TEXCOORD0)
        LIL_V2F_OUT_BASE.uv0            = input.uv0;
    #endif
    #if defined(LIL_V2F_TEXCOORD1)
        LIL_V2F_OUT_BASE.uv1            = input.uv1;
    #endif
    #if defined(LIL_V2F_TEXCOORD2)
        LIL_V2F_OUT_BASE.uv2            = input.uv2;
    #endif
    #if defined(LIL_V2F_TEXCOORD3)
        LIL_V2F_OUT_BASE.uv3            = input.uv3;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD01)
        LIL_V2F_OUT_BASE.uv01.xy        = input.uv0;
        LIL_V2F_OUT_BASE.uv01.zw        = input.uv1;
    #endif
    #if defined(LIL_V2F_PACKED_TEXCOORD23)
        LIL_V2F_OUT_BASE.uv23.xy        = input.uv2;
        LIL_V2F_OUT_BASE.uv23.zw        = input.uv3;
    #endif
    #if defined(LIL_V2F_UVMAT)
        LIL_V2F_OUT_BASE.uvMat          = lilCalcMatCapUV(input.uv1, vertexNormalInput.normalWS, viewDirection, headDirection, _MatCapTex_ST, _MatCapBlendUV1.xy, _MatCapZRotCancel, _MatCapPerspective, _MatCapVRParallaxStrength);
    #endif

    // Position
    #if defined(LIL_V2F_POSITION_CS)
        LIL_V2F_OUT_BASE.positionCS     = vertexInput.positionCS;
    #endif
    #if defined(LIL_V2F_POSITION_OS)
        LIL_V2F_OUT_BASE.positionOS     = input.positionOS.xyz;
    #endif
    #if defined(LIL_V2F_POSITION_WS)
        LIL_V2F_OUT_BASE.positionWS     = vertexInput.positionWS;
    #endif

    // Normal
    #if defined(LIL_V2F_NORMAL_WS) && defined(LIL_NORMALIZE_NORMAL_IN_VS) && !defined(SHADER_QUALITY_LOW)
        LIL_V2F_OUT_BASE.normalWS       = normalize(vertexNormalInput.normalWS);
    #elif defined(LIL_V2F_NORMAL_WS)
        LIL_V2F_OUT_BASE.normalWS       = vertexNormalInput.normalWS;
    #endif
    #if defined(LIL_V2F_TANGENT_WS)
        LIL_V2F_OUT_BASE.tangentWS      = float4(vertexNormalInput.tangentWS, input.tangentOS.w);
    #endif

    LIL_CUSTOM_VERT_COPY

    //------------------------------------------------------------------------------------------------------------------------------
    // Meta
    #if defined(LIL_PASS_META_INCLUDED) && !defined(LIL_HDRP)
        LIL_TRANSFER_METAPASS(input,LIL_V2F_OUT_BASE);
        #if defined(EDITOR_VISUALIZATION)
            if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
                LIL_V2F_OUT_BASE.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, input.uv0, input.uv1, input.uv2, unity_EditorViz_Texture_ST);
            else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
            {
                LIL_V2F_OUT_BASE.vizUV = input.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
                LIL_V2F_OUT_BASE.lightCoord = mul(unity_EditorViz_WorldToLight, float4(lilTransformOStoWS(input.positionOS.xyz), 1.0));
            }
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Fog & Lighting
    lilFragData fd = lilInitFragData();
    LIL_GET_HDRPDATA(vertexInput,fd);
    #if defined(LIL_V2F_LIGHTCOLOR) || defined(LIL_V2F_LIGHTDIRECTION) || defined(LIL_V2F_INDLIGHTCOLOR) || defined(LIL_V2F_NDOTL)
        LIL_CALC_MAINLIGHT(vertexInput, lightdataInput);
    #endif
    #if defined(LIL_V2F_LIGHTCOLOR)
        LIL_V2F_OUT_BASE.lightColor     = lightdataInput.lightColor;
    #endif
    #if defined(LIL_V2F_LIGHTDIRECTION)
        LIL_V2F_OUT_BASE.lightDirection = lightdataInput.lightDirection;
    #endif
    #if defined(LIL_V2F_INDLIGHTCOLOR)
        LIL_V2F_OUT_BASE.indLightColor  = lightdataInput.indLightColor * _ShadowEnvStrength;
    #endif
    #if defined(LIL_V2F_NDOTL)
        float2 outlineNormalVS = normalize(lilTransformDirWStoVSCenter(vertexNormalInput.normalWS).xy);
        #if defined(LIL_PASS_FORWARDADD)
            float2 outlineLightVS = normalize(lilTransformDirWStoVSCenter(normalize(_WorldSpaceLightPos0.xyz - vertexInput.positionWS * _WorldSpaceLightPos0.w)).xy);
        #else
            float2 outlineLightVS = normalize(lilTransformDirWStoVSCenter(lightdataInput.lightDirection).xy);
        #endif
        LIL_V2F_OUT_BASE.NdotL          = dot(outlineNormalVS, outlineLightVS) * 0.5 + 0.5;
    #endif
    #if defined(LIL_V2F_SHADOW)
        LIL_TRANSFER_SHADOW(vertexInput, input.uv1, LIL_V2F_OUT_BASE);
    #endif
    #if defined(LIL_V2F_VERTEXLIGHT_FOG)
        LIL_TRANSFER_FOG(vertexInput, LIL_V2F_OUT_BASE);
        LIL_CALC_VERTEXLIGHT(vertexInput, LIL_V2F_OUT_BASE);
    #endif
    #if defined(LIL_V2F_SHADOW_CASTER)
        LIL_TRANSFER_SHADOW_CASTER(input, LIL_V2F_OUT_BASE);
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Clipping Canceller
    #if defined(LIL_V2F_POSITION_CS) && defined(LIL_FEATURE_CLIPPING_CANCELLER) && !defined(LIL_LITE) && !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !defined(LIL_PASS_META_INCLUDED)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            if(LIL_V2F_OUT_BASE.positionCS.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT_BASE.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
            {
                LIL_V2F_OUT_BASE.positionCS.z = LIL_V2F_OUT_BASE.positionCS.z * 0.0001 + LIL_V2F_OUT_BASE.positionCS.w * 0.999;
            }
        #else
            // OpenGL
            if(LIL_V2F_OUT_BASE.positionCS.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT_BASE.positionCS.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
            {
                LIL_V2F_OUT_BASE.positionCS.z = LIL_V2F_OUT_BASE.positionCS.z * 0.0001 - LIL_V2F_OUT_BASE.positionCS.w * 0.999;
            }
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // One Pass Outline (for HDRP)
    #if defined(LIL_ONEPASS_OUTLINE)
        #include "lil_vert_outline.hlsl"
        vertexInput = lilGetVertexPositionInputs(input.positionOS);
        lilCustomVertexWS(input, uvMain, vertexInput, vertexNormalInput);
        #if defined(LIL_CUSTOM_VERTEX_WS)
            LIL_RE_VERTEX_POSITION_INPUTS(vertexInput);
        #endif
        LIL_V2F_OUT.positionCSOL = vertexInput.positionCS;

        //------------------------------------------------------------------------------------------------------------------------------
        // Clipping Canceller
        #if defined(LIL_FEATURE_CLIPPING_CANCELLER) && !defined(LIL_LITE) && !defined(LIL_PASS_SHADOWCASTER_INCLUDED) && !defined(LIL_PASS_META_INCLUDED)
            #if defined(UNITY_REVERSED_Z)
                // DirectX
                if(LIL_V2F_OUT.positionCSOL.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT.positionCSOL.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
                {
                    LIL_V2F_OUT.positionCSOL.z = LIL_V2F_OUT.positionCSOL.z * 0.0001 + LIL_V2F_OUT.positionCSOL.w * 0.999;
                }
            #else
                // OpenGL
                if(LIL_V2F_OUT.positionCSOL.w < _ProjectionParams.y * 1.01 && LIL_V2F_OUT.positionCSOL.w > 0 && _ProjectionParams.y < LIL_NEARCLIP_THRESHOLD LIL_MULTI_SHOULD_CLIPPING)
                {
                    LIL_V2F_OUT.positionCSOL.z = LIL_V2F_OUT.positionCSOL.z * 0.0001 - LIL_V2F_OUT.positionCSOL.w * 0.999;
                }
            #endif
        #endif

        //------------------------------------------------------------------------------------------------------------------------------
        // Offset z for Less ZTest
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            LIL_V2F_OUT.positionCSOL.z -= 0.0001;
        #else
            // OpenGL
            LIL_V2F_OUT.positionCSOL.z += 0.0001;
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Offset z for Less ZTest
    #if defined(SHADERPASS) && SHADERPASS == SHADERPASS_DEPTH_ONLY && defined(LIL_OUTLINE)
        #if defined(UNITY_REVERSED_Z)
            // DirectX
            LIL_V2F_OUT.positionCS.z -= 0.0001;
        #else
            // OpenGL
            LIL_V2F_OUT.positionCS.z += 0.0001;
        #endif
    #endif

    //------------------------------------------------------------------------------------------------------------------------------
    // Remove Outline
    #if defined(LIL_ONEPASS_OUTLINE)
        float width = lilGetOutlineWidth(uvMain, input.color, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width LIL_SAMP_IN(lil_sampler_linear_repeat));
        if(abs(width) < 0.000001 && _OutlineDeleteMesh) LIL_V2F_OUT.positionCSOL = 0.0/0.0;
    #elif defined(LIL_OUTLINE)
        float width = lilGetOutlineWidth(uvMain, input.color, _OutlineWidth, _OutlineWidthMask, _OutlineVertexR2Width LIL_SAMP_IN(lil_sampler_linear_repeat));
        if(abs(width) < 0.000001 && _OutlineDeleteMesh) LIL_V2F_OUT.positionCS = 0.0/0.0;
    #endif

    #if defined(LIL_TESSELLATION) || defined(LIL_CUSTOM_SAFEVERT)
        }
    #endif

    return LIL_V2F_OUT;
}


#if defined(LIL_ONEPASS_OUTLINE)
    [maxvertexcount(12)]
    void geomOpt(triangle v2g input[3], inout TriangleStream<v2f> outStream)
    {
        geom(input, outStream);
    }
#endif


#undef LIL_V2F_OUT_BASE
#undef LIL_V2F_OUT
#undef LIL_V2F_TYPE


#endif  // defined(LIL_PASS_FORWARD_FUR_INCLUDED)


#if defined(LIL_TESSELLATION_INCLUDED)
//------------------------------------------------------------------------------------------------------------------------------
// Hull Shader
[domain("tri")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[patchconstantfunc("hullConstOpt")]
[outputcontrolpoints(3)]
appdata hullOpt(InputPatch<appdata, 3> input, uint id : SV_OutputControlPointID)
{
    return input[id];
}

lilTessellationFactors hullConstOpt(InputPatch<appdata, 3> input)
{
    lilTessellationFactors output;
    LIL_INITIALIZE_STRUCT(lilTessellationFactors, output);
    LIL_SETUP_INSTANCE_ID(input[0]);

    if(_Invisible) return output;

    LIL_VERTEX_POSITION_INPUTS(input[0].positionOS, vertexInput_0);
    LIL_VERTEX_POSITION_INPUTS(input[1].positionOS, vertexInput_1);
    LIL_VERTEX_POSITION_INPUTS(input[2].positionOS, vertexInput_2);
    LIL_VERTEX_NORMAL_INPUTS(input[0].normalOS, vertexNormalInput_0);
    LIL_VERTEX_NORMAL_INPUTS(input[1].normalOS, vertexNormalInput_1);
    LIL_VERTEX_NORMAL_INPUTS(input[2].normalOS, vertexNormalInput_2);

    float4 tessFactor;
    tessFactor.x = lilCalcEdgeTessFactor(vertexInput_1.positionWS, vertexInput_2.positionWS, _TessEdge);
    tessFactor.y = lilCalcEdgeTessFactor(vertexInput_2.positionWS, vertexInput_0.positionWS, _TessEdge);
    tessFactor.z = lilCalcEdgeTessFactor(vertexInput_0.positionWS, vertexInput_1.positionWS, _TessEdge);
    tessFactor.xyz = min(tessFactor.xyz, _TessFactorMax);

    // Rim
    float3 nv = float3(abs(dot(vertexNormalInput_0.normalWS, lilViewDirection(lilToAbsolutePositionWS(vertexInput_0.positionWS)))),
                       abs(dot(vertexNormalInput_1.normalWS, lilViewDirection(lilToAbsolutePositionWS(vertexInput_1.positionWS)))),
                       abs(dot(vertexNormalInput_2.normalWS, lilViewDirection(lilToAbsolutePositionWS(vertexInput_2.positionWS)))));
    nv = saturate(1.0 - float3(nv.y + nv.z, nv.z + nv.x, nv.x + nv.y) * 0.5);
    tessFactor.xyz = max(tessFactor.xyz * nv * nv, 1.0);
    tessFactor.w = dot(tessFactor.xyz, 1.0 / 3.0);

    // Cull out of screen
    float4 pos[3] = {vertexInput_0.positionCS, vertexInput_1.positionCS, vertexInput_2.positionCS};
    pos[0].xy = pos[0].xy/abs(pos[0].w);
    pos[1].xy = pos[1].xy/abs(pos[1].w);
    pos[2].xy = pos[2].xy/abs(pos[2].w);
    tessFactor = (pos[0].x >  1.01 && pos[1].x >  1.01 && pos[2].x >  1.01) ||
                 (pos[0].x < -1.01 && pos[1].x < -1.01 && pos[2].x < -1.01) ||
                 (pos[0].y >  1.01 && pos[1].y >  1.01 && pos[2].y >  1.01) ||
                 (pos[0].y < -1.01 && pos[1].y < -1.01 && pos[2].y < -1.01) ? 0.0 : tessFactor;

    output.edge[0] = tessFactor.x;
    output.edge[1] = tessFactor.y;
    output.edge[2] = tessFactor.z;
    output.inside  = tessFactor.w;

    return output;
}


//------------------------------------------------------------------------------------------------------------------------------
// Domain Shader
[domain("tri")]
#if defined(LIL_ONEPASS_OUTLINE)
v2g domainOpt(lilTessellationFactors hsConst, const OutputPatch<appdata, 3> input, float3 bary : SV_DomainLocation)
#else
v2f domainOpt(lilTessellationFactors hsConst, const OutputPatch<appdata, 3> input, float3 bary : SV_DomainLocation)
#endif
{
    appdata output;
    LIL_INITIALIZE_STRUCT(appdata, output);
    LIL_TRANSFER_INSTANCE_ID(input[0], output);

    #if defined(LIL_APP_POSITION)
        LIL_TRI_INTERPOLATION(input,output,bary,positionOS);
    #endif
    #if defined(LIL_APP_TEXCOORD0)
        LIL_TRI_INTERPOLATION(input,output,bary,uv0);
    #endif
    #if defined(LIL_APP_TEXCOORD1)
        LIL_TRI_INTERPOLATION(input,output,bary,uv1);
    #endif
    #if defined(LIL_APP_TEXCOORD2)
        LIL_TRI_INTERPOLATION(input,output,bary,uv2);
    #endif
    #if defined(LIL_APP_TEXCOORD3)
        LIL_TRI_INTERPOLATION(input,output,bary,uv3);
    #endif
    #if defined(LIL_APP_TEXCOORD4)
        LIL_TRI_INTERPOLATION(input,output,bary,uv4);
    #endif
    #if defined(LIL_APP_TEXCOORD5)
        LIL_TRI_INTERPOLATION(input,output,bary,uv5);
    #endif
    #if defined(LIL_APP_TEXCOORD6)
        LIL_TRI_INTERPOLATION(input,output,bary,uv6);
    #endif
    #if defined(LIL_APP_TEXCOORD7)
        LIL_TRI_INTERPOLATION(input,output,bary,uv7);
    #endif
    #if defined(LIL_APP_COLOR)
        LIL_TRI_INTERPOLATION(input,output,bary,color);
    #endif
    #if defined(LIL_APP_NORMAL)
        LIL_TRI_INTERPOLATION(input,output,bary,normalOS);
    #endif
    #if defined(LIL_APP_TANGENT)
        LIL_TRI_INTERPOLATION(input,output,bary,tangentOS);
    #endif
    #if defined(LIL_APP_VERTEXID)
        output.vertexID = input[0].vertexID;
    #endif
    #if defined(LIL_APP_PREVPOS)
        LIL_TRI_INTERPOLATION(input,output,bary,previousPositionOS);
    #endif
    #if defined(LIL_APP_PREVEL)
        LIL_TRI_INTERPOLATION(input,output,bary,precomputedVelocity);
    #endif

    output.normalOS = normalize(output.normalOS);
    float3 pt[3];
    for(int i = 0; i < 3; i++)
        pt[i] = input[i].normalOS * (dot(input[i].positionOS.xyz, input[i].normalOS) - dot(output.positionOS.xyz, input[i].normalOS) - _TessShrink*0.01);
    output.positionOS.xyz += (pt[0] * bary.x + pt[1] * bary.y + pt[2] * bary.z) * _TessStrength;

    return vertOpt(output);
}

#endif



#endif  // LIL_OPT_VERT_INCLUDED
