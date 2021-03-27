Shader "Custom/Standard/Bllin Phong"
{
    Properties
    {
        _AlbedoTex ("Albedo", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _SpecularTint ("Specular Tint", Color) = (1,1,1,1)
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
            
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            
            #include "UnityStandardBRDF.cginc"
            #include "UnityStandardUtils.cginc"   
            
            sampler2D _AlbedoTex;
            float4 _AlbedoTex_ST;
                
            float _Smoothness ;        
            float4 _Tint;
            float4 _SpecularTint;
            
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
                
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;  
                
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - interpolators.worldPos);
                
                // With Bllin reflection model 
                //float3 reflectionDir = reflect(-lightDir,interpolators.normal);
                //float3 specular = pow( DotClamped(viewDir,reflectionDir), _Smoothness * 100.0f);
                
                // With Bllin-Phong reflection model 
                float3 halfVector = normalize(lightDir + viewDir);
                float3 specular = pow( DotClamped(viewDir,interpolators.normal), _Smoothness * 100.0f) * _SpecularTint;

                float3 albedoTexture = tex2D(_AlbedoTex,interpolators.uv) * _Tint;
                float oneMinusReflectivity;
                
                //other way to correctly compute energy albedo *= 1 - max(_SpecularTint.r, max(_SpecularTint.g, _SpecularTint.b));
				albedoTexture = EnergyConservationBetweenDiffuseAndSpecular(
					albedoTexture, _SpecularTint.rgb, oneMinusReflectivity
				);
                       
                float3 diffuse = albedoTexture * lightColor * DotClamped(lightDir,interpolators.normal);
                return float4(diffuse + specular,1);
            }
            
            ENDCG
        }
    }
}
