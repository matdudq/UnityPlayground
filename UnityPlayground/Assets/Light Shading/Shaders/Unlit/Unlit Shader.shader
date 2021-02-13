Shader "Custom/Unlit/Unlit Shader"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            
            #include "UnityCG.cginc"
            
            float4 _Tint;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};
            
            struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
            
            Interpolators VertexProgram(VertexData vertexData)
            {
                Interpolators interpolators;
                interpolators.position = UnityObjectToClipPos(vertexData.position);
                interpolators.uv = vertexData.uv * _MainTex_ST.xy + _MainTex_ST.zw; //Same as TRANSFORM_TEX(v.uv, _MainTex);
                return interpolators;
            }
            
            float4 FragmentProgram(Interpolators interpolators) : SV_TARGET
            {
                return tex2D(_MainTex,interpolators.uv) * _Tint;
            }
            
            ENDCG
        }
    }
}
