float3 getEmissionPos(float3 positionOS)
{
#if defined(_WAVEPOSSPACE_OBJECT)
    return positionOS;
#else
    return mul((float3x3)unity_ObjectToWorld, positionOS)
        / float3(
            length(unity_ObjectToWorld._m00_m10_m20),
            length(unity_ObjectToWorld._m01_m11_m21),
            length(unity_ObjectToWorld._m02_m12_m22));
#endif
}
