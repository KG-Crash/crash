Shader "Content/UnitToMinimap"
{
    Properties { }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vertex
            #pragma fragment pixel
            
            #include "UnityCG.cginc"

            uniform float4 _UnitPositions[500 * 4 / 2];
            uniform float4 _UnitColors[10];
            uniform float _UnitColorIndices[500];
            
            struct Varying
            {
                float4 pos : SV_POSITION;
                uint instanceIndex : SV_InstanceID;
            };
            
            Varying vertex(uint vertexIndex : SV_VertexID, uint instanceIndex : SV_InstanceID)
            {
                Varying v;
                float4 dualPos = _UnitPositions[instanceIndex * 2 + vertexIndex / 2] * 2 - 1;
                v.pos = float4(
                    dualPos.xy * (vertexIndex % 2) + dualPos.zw * ((vertexIndex + 1) % 2), 0, 1
                );
                v.instanceIndex = instanceIndex;
                return v;
            }
            
            fixed4 pixel(Varying v) : SV_Target
            {
                return _UnitColors[_UnitColorIndices[v.instanceIndex]];
            }
            ENDHLSL
        }
    }
}