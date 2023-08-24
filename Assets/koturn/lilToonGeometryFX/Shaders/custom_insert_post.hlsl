#if !defined(LIL_PASS_FORWARD_FUR_INCLUDED)
//------------------------------------------------------------------------------------------------------------------------------
// Vertex Shader
appdataCopy vertCustom(appdata i){ return appdataOriginalToCopy(i); }

//------------------------------------------------------------------------------------------------------------------------------
// Domain Shader
#if defined(LIL_TESSELLATION_INCLUDED)
[domain("tri")]
appdataCopy domainCustom(lilTessellationFactors hsConst, const OutputPatch<appdata, 3> input, float3 bary : SV_DomainLocation)
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

    return appdataOriginalToCopy(output);
}
#endif

//------------------------------------------------------------------------------------------------------------------------------
// Geometry Shader
void GenerateTop(appdata i_o[3], v2f base_o[3], float3 triNormal, float3 quadNormal, inout TriangleStream<v2f> outStream)
{
    #if defined(LIL_V2F_NORMAL_WS)
        if(_CustomGeometryShadingNormal)
        {
            float3 flatNormal = _CustomGeometryShadingNormal == 2 ? quadNormal : triNormal;
            LIL_VERTEX_NORMAL_INPUTS(flatNormal, vertexNormalInput);
            #if defined(LIL_NORMALIZE_NORMAL_IN_VS) && !defined(SHADER_QUALITY_LOW)
                vertexNormalInput.normalWS = normalize(vertexNormalInput.normalWS);
            #endif
            base_o[0].normalWS = vertexNormalInput.normalWS;
            base_o[1].normalWS = vertexNormalInput.normalWS;
            base_o[2].normalWS = vertexNormalInput.normalWS;
        }
    #endif
    outStream.Append(base_o[0]);
    outStream.Append(base_o[1]);
    outStream.Append(base_o[2]);
    outStream.RestartStrip();
}

void GenerateSide(appdata i0, appdata i1, appdata i2, appdata i3, v2f base0, v2f base1, v2f base2, v2f base3, inout TriangleStream<v2f> outStream)
{
    #if defined(LIL_V2F_NORMAL_WS)
        float3 sideNormal = normalize(cross(i1.positionOS.xyz - i0.positionOS.xyz, i3.positionOS.xyz - i1.positionOS.xyz));
        LIL_VERTEX_NORMAL_INPUTS(sideNormal, vertexNormalInput);
        #if defined(LIL_V2F_NORMAL_WS)
            #if defined(LIL_NORMALIZE_NORMAL_IN_VS) && !defined(SHADER_QUALITY_LOW)
                vertexNormalInput.normalWS = normalize(vertexNormalInput.normalWS);
            #endif
            base0.normalWS = vertexNormalInput.normalWS;
            base1.normalWS = vertexNormalInput.normalWS;
            base2.normalWS = vertexNormalInput.normalWS;
            base3.normalWS = vertexNormalInput.normalWS;
        #endif
    #endif
    outStream.Append(base0);
    outStream.Append(base1);
    outStream.Append(base2);
    outStream.Append(base3);
    outStream.RestartStrip();
}

[maxvertexcount(15)]
void geomCustom(triangle appdataCopy ic[3], uint primitiveID : SV_PrimitiveID, inout TriangleStream<v2f> outStream)
{
    if(_Invisible) return;

    appdata i[3] = {appdataOriginalToCopy(ic[0]), appdataOriginalToCopy(ic[1]), appdataOriginalToCopy(ic[2])};

    LIL_SETUP_INSTANCE_ID(i[0]);
    float3 triNormal = normalize(i[0].normalOS + i[1].normalOS + i[2].normalOS);
    float3 quadNormal = normalize(cross(i[1].positionOS.xyz - i[0].positionOS.xyz, i[2].positionOS.xyz - i[0].positionOS.xyz));
    float3 normalOS = _CustomGeometryMotionNormal ? quadNormal : triNormal;
    float3 tangentOS = normalize(i[0].tangentOS.xyz + i[1].tangentOS.xyz + i[2].tangentOS.xyz);
    float3 bitangentOS = normalize(cross(normalOS, tangentOS.xyz) * i[0].tangentOS.w);
    float3x3 tbnOS = float3x3(tangentOS, bitangentOS, normalOS);
    float3 positionOS = (i[0].positionOS.xyz + i[1].positionOS.xyz + i[2].positionOS.xyz) * 0.333333;
    float2 uv0 = (i[0].uv0.xy + i[1].uv0.xy + i[2].uv0.xy) * 0.333333;

    float4 geometryMask = LIL_SAMPLE_2D_LOD(_CustomGeometryMask, sampler_linear_repeat, uv0, 0);

    float3 animation = 0.0;
                                 animation.xy  = uv0;
    if(_CustomGeometryMode == 1) animation.xy  = (i[0].uv1.xy + i[1].uv1.xy + i[2].uv1.xy) * 0.333333;
    if(_CustomGeometryMode == 2) animation.xy  = (i[0].uv2.xy + i[1].uv2.xy + i[2].uv2.xy) * 0.333333;
    if(_CustomGeometryMode == 3) animation.xy  = (i[0].uv3.xy + i[1].uv3.xy + i[2].uv3.xy) * 0.333333;
    if(_CustomGeometryMode == 4) animation.xyz = positionOS.xyz;
    if(_CustomGeometryMode == 5) animation.xyz = lilTransformOStoWS(positionOS.xyz);
    if(_CustomGeometryMode == 6) animation.xyz = normalOS;
    if(_CustomGeometryMode == 7) animation.xyz = geometryMask.rgb;

    float animationScale = sin(
        LIL_TIME * _CustomGeometrySpeed +
        dot(animation, _CustomGeometryVector.xyz) +
        _CustomGeometryVector.w +
        frac(sin(primitiveID * 12.9898) * 43758.5453123) * LIL_TWO_PI * _CustomGeometryRandomize
    );
    animationScale = clamp(animationScale, _CustomGeometryMin, _CustomGeometryMax);
    animationScale *= geometryMask.a;

    float3 commonMotion = 0.0;

    // Local Normal & Normal Map
    float3 normalVector = lilUnpackNormalScale(LIL_SAMPLE_2D_LOD(_CustomGeometryNormalMap, sampler_linear_repeat, uv0, 0), _CustomGeometryNormalMapScale) * _CustomGeometryNormalMapStrength;
    normalVector += _CustomGeometryNormalOffset.xyz;
    commonMotion += mul(normalVector, tbnOS)                                     * (animationScale + _CustomGeometryNormalOffset.w);
    commonMotion +=                       _CustomGeometryLocalOffset.xyz         * (animationScale + _CustomGeometryLocalOffset.w);
    commonMotion += lilTransformDirWStoOS(_CustomGeometryWorldOffset.xyz, false) * (animationScale + _CustomGeometryWorldOffset.w);

    float3 motion[3] = {commonMotion,commonMotion,commonMotion};

    // Shrink
    motion[0] += (positionOS - i[0].positionOS.xyz) * (animationScale * _CustomGeometryShrinkStrength + _CustomGeometryShrinkOffset);
    motion[1] += (positionOS - i[1].positionOS.xyz) * (animationScale * _CustomGeometryShrinkStrength + _CustomGeometryShrinkOffset);
    motion[2] += (positionOS - i[2].positionOS.xyz) * (animationScale * _CustomGeometryShrinkStrength + _CustomGeometryShrinkOffset);

    // Apply
    appdata i_o[3] = i;
    i_o[0].positionOS.xyz += motion[0];
    i_o[1].positionOS.xyz += motion[1];
    i_o[2].positionOS.xyz += motion[2];

    v2f base_o[3] = {vert(i_o[0]), vert(i_o[1]), vert(i_o[2])};

    GenerateTop(i_o, base_o, triNormal, quadNormal, outStream);

    if(_CustomGeometryGenerateSide)
    {
        v2f base[3] = {vert(i[0]), vert(i[1]), vert(i[2])};
        GenerateSide(i_o[0], i[0], i_o[1], i[1], base_o[0], base[0], base_o[1], base[1], outStream);
        GenerateSide(i_o[1], i[1], i_o[2], i[2], base_o[1], base[1], base_o[2], base[2], outStream);
        GenerateSide(i_o[2], i[2], i_o[0], i[0], base_o[2], base[2], base_o[0], base[0], outStream);
    }
}
#endif
