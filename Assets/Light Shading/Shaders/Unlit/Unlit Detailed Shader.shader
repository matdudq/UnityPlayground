Shader "Custom/Unlit/Unlit Detailed Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DetailTex ("Detail texture", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            
            #include "UnityCG.cginc"
                        
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _DetailTex;
            float4 _DetailTex_ST;
            
            struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};
            
            struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvDetail : TEXCOORD1;
			};
            
            Interpolators VertexProgram(VertexData vertexData)
            {
                Interpolators interpolators;
                interpolators.position = UnityObjectToClipPos(vertexData.position);
                interpolators.uv = TRANSFORM_TEX(vertexData.uv, _MainTex);
                interpolators.uvDetail = TRANSFORM_TEX(vertexData.uv, _DetailTex);
                return interpolators;
            }
            
            float4 FragmentProgram(Interpolators interpolators) : SV_TARGET
            {
                return tex2D(_MainTex,interpolators.uv) * 
                       tex2D(_DetailTex,interpolators.uvDetail) *
                       unity_ColorSpaceDouble;
            }
            
            ENDCG
        }
    }
}
