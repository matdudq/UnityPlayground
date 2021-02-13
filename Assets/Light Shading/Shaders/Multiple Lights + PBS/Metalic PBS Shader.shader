Shader "Custom/Standard/Metalic PBS"
{
    Properties
    {
        _AlbedoTex ("Albedo", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        [Gamma] _Metallic ("Metalic", Range(0,1)) = 0
        _Smoothness ("_Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        Pass
        {
            Tags
            {
            "LightMode" = "ForwardBase"
            }
        
            CGPROGRAM
            
            #pragma target 3.0
            
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            
            #include "UnityPBSLighting.cginc"  
            
            sampler2D _AlbedoTex;
            float4 _AlbedoTex_ST;
                
            float _Smoothness;     
            float _Metallic;   
            float4 _Tint;
            
            struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
			};
            
            struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
				float3 worldPos: TEXCOORD1;
			};
            
            Interpolators VertexProgram(VertexData vertexData)
            {
                Interpolators interpolators;
                interpolators.position = UnityObjectToClipPos(vertexData.position);
                interpolators.worldPos = mul(unity_ObjectToWorld,vertexData.position);
                interpolators.uv = TRANSFORM_TEX(vertexData.uv, _AlbedoTex);
                interpolators.normal = UnityObjectToWorldNormal(vertexData.normal); // normalize(mul(transpose((float3x3)unity_WorldToObject),vertexData.normal))
                return interpolators;
            }
            
            float4 FragmentProgram(Interpolators interpolators) : SV_TARGET
            {
                interpolators.normal = normalize(interpolators.normal);
                                
                float3 viewDir = normalize(_WorldSpaceCameraPos - interpolators.worldPos);
                
                float oneMinusReflectivity;
                float3 speculatTint;
                float3 albedoTexture = tex2D(_AlbedoTex,interpolators.uv) * _Tint;
                   
                albedoTexture = DiffuseAndSpecularFromMetallic(albedoTexture,_Metallic,speculatTint,oneMinusReflectivity); 
                
                
                UnityLight light;
                light.color = _LightColor0.rgb;  
                light.dir = _WorldSpaceLightPos0.xyz;
                light.ndotl = DotClamped(interpolators.normal, _WorldSpaceLightPos0.xyz);
                
                UnityIndirect indirectLight;
                indirectLight.diffuse = 0;
                indirectLight.specular = 0;
                
                return UNITY_BRDF_PBS(albedoTexture, speculatTint,
                                      oneMinusReflectivity, _Smoothness,
                                      interpolators.normal, viewDir,
                                      light, indirectLight);
            }
            
            ENDCG
        }
    }
}
