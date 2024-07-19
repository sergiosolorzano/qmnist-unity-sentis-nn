Shader "Hidden/Sentis/Softmax"
{
    Properties
    {
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma multi_compile SOFTMAXEND LOGSOFTMAXEND

            #pragma vertex vert
            #pragma fragment frag

            #include "CommonVertexShader.cginc"
            #include "CommonPixelShader.cginc"

            DECLARE_TENSOR(X);
            DECLARE_TENSOR(S);
            DECLARE_TENSOR(B);

            uint StrideAxisX, DimAxisX;

            float4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {
                uint blockIndexO = GetBlockIndexO(screenPos);
                uint2 lowerUpper = Unravel(uint1(StrideAxisX), blockIndexO);
                lowerUpper[1] /= DimAxisX;
                uint blockIndexSB = Ravel(uint1(StrideAxisX), lowerUpper);
                float4 x = SampleBlockX(blockIndexO);
                float4 s = SampleBlockS(blockIndexSB);
                float4 b = SampleBlockB(blockIndexSB);
                #ifdef LOGSOFTMAXEND
                float4 v = (x - b) - log(s);
                #else // SOFTMAXEND
                float4 v = exp(x - b) / s;
                #endif
                return v;
            }
            ENDCG
        }
    }
}
