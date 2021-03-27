#include "UnityStandardBRDF.cginc"
#include "UnityStandardUtils.cginc"  
 
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"
   
#define PI 3.14159265
            
sampler2D _AlbedoTex;
float4 _AlbedoTex_ST;
sampler2D _DiffuseHDR;
float4 _DiffuseHDR_ST;
sampler2D _SpecularHDR;
float4 _SpecularHDR_ST;

sampler2D _NormalMap;
 
float _Smoothness;        
float _DiffuseIBLIntensity;        
float _SpecularIBLIntensity;        
float _FresnelIntensity;        
float4 _Tint;
float4 _SpecularTint;

struct VertexData {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal: NORMAL;
};

struct Interpolators {
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 normal: TEXCOORD1;
    float3 worldPos: TEXCOORD2;
    LIGHTING_COORDS(3,4)
};

Interpolators VertexProgram(VertexData v)
{
    Interpolators interpolators;
    interpolators.position = UnityObjectToClipPos(v.vertex);
    interpolators.worldPos = mul(unity_ObjectToWorld,v.vertex);
    interpolators.uv = TRANSFORM_TEX(v.uv, _AlbedoTex);
    interpolators.normal = UnityObjectToWorldNormal(v.normal); // normalize(mul(transpose((float3x3)unity_WorldToObject),vertexData.normal))
    TRANSFER_VERTEX_TO_FRAGMENT(interpolators);
    return interpolators;
}

float2 DirToRectilinear(float3 direction)
{
    return float2( atan2(direction.z,direction.x) / PI , direction.y * 0.5 + 0.5);
}

float4 FragmentProgram(Interpolators interpolators) : SV_TARGET
{
    
    float3 normal = normalize(UnityWorldSpaceLightDir(interpolators.normal));
    
    float3 lightDir = _WorldSpaceLightPos0.xyz;  
    
    float3 viewDir = normalize(_WorldSpaceCameraPos - interpolators.worldPos);
    
    // With Bllin reflection model 
    //float3 reflectionDir = reflect(-lightDir,normal);
    //float3 specular = pow( DotClamped(viewDir,reflectionDir), _Smoothness * 100.0f);
    
    // With Bllin-Phong reflection model 
    float3 halfVector = normalize(lightDir + viewDir);
    float3 specular = pow( DotClamped(viewDir,normal), _Smoothness * 100.0f) * _SpecularTint;
    
    #ifdef IS_IN_BASE_PASS

    float3 viewReflected = reflect(-viewDir,normal);
    float mip = (1-_Smoothness)*5;
    float specularIBL = tex2Dlod(_SpecularHDR, float4(DirToRectilinear(viewReflected),0,0)).xyz;
    
    float fresnelMask = pow(1-saturate(dot(viewDir,normal)),_FresnelIntensity * 5 );
    
    specular += specularIBL * _SpecularIBLIntensity * fresnelMask;
    
    #endif 
    
    float3 albedoTexture = tex2D(_AlbedoTex,interpolators.uv) * _Tint;
    float oneMinusReflectivity;
    
    //other way to correctly compute energy albedo *= 1 - max(_SpecularTint.r, max(_SpecularTint.g, _SpecularTint.b));
    albedoTexture = EnergyConservationBetweenDiffuseAndSpecular(
        albedoTexture, _SpecularTint.rgb, oneMinusReflectivity
    );
           
    float3 diffuse =  _LightColor0.rgb * DotClamped(lightDir,normal);
    
    #ifdef IS_IN_BASE_PASS
    
    float3 diffuseIBL = tex2Dlod(_DiffuseHDR, float4(DirToRectilinear(normal),0,0)).xyz;
    diffuse += diffuseIBL * _DiffuseIBLIntensity;
    
    #endif   
          
    return float4(saturate(diffuse * specular + albedoTexture),1);
}