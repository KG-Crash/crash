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

            uniform float4 _UnitPositions[1000];
            uniform float4 _UnitColors[10];
            uniform float _UnitColorIndices[500];
            
            float4 vertex(uint vertexIndex : SV_VertexID, uint instanceIndex : SV_InstanceID) : SV_POSITION
            {
                float4 dualPos = _UnitPositions[instanceIndex * 2 + vertexIndex / 2] * 2 - 1;
                return float4(
                    dualPos.xy * (vertexIndex % 2) + dualPos.zw * ((vertexIndex + 1) % 2), 0, 1
                );
            }
            
            fixed4 pixel(float4 pos : SV_POSITION, uint instanceIndex : SV_InstanceID) : SV_Target
            {
                return _UnitColors[(uint)_UnitColorIndices[instanceIndex]];
            }
            ENDHLSL
        }
    }
}