Shader "Custom/Skybox/Rectlinear"
{
    Properties
    {
        _MainTex ("Skybox texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Back
                   
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define PI 3.14159265

            float2 DirToRectilinear(float3 direction)
            {
                return float2( atan2(direction.z,direction.x) / PI , direction.y * 0.5 + 0.5);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {                
                return tex2Dlod(_MainTex, float4(DirToRectilinear(i.uv),0,0));
            }
                   
            ENDCG
        }
    }
}
