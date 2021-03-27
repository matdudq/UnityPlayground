#pragma vertex VertexProgram
#pragma fragment FragmentProgram

#if !defined(LIGHTING_INCLUDED)
#define LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"  

sampler2D _AlbedoTex;
float4 _AlbedoTex_ST;
    
float _Smoothness;     
float _Metallic;   
float4 _Tint;

sampler2D _NormalMap, _DetailNormalMap;
float _BumpScale, _DetailBumpScale;

sampler2D _DetailTex;
float4 _DetailTex_ST;

struct VertexData {
    float4 position : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal: NORMAL;
    float4 tangent: TANGENT;
};

struct Interpolators {
    float4 position : SV_POSITION;
    float4 uv : TEXCOORD0;
    float3 normal: TEXCOORD1;
    
    #if defined(BINORMAL_PER_FRAGMENT)
    float4 tangent: TEXCOORD2;
    #else
    float3 tangent: TEXCOORD2;
    float3 binormal: TEXCOORD3;
    #endif
        
    float3 worldPos: TEXCOORD4;
    
    #if defined(VERTEXLIGHT_ON)
    float3 vertexLightColor : TEXCOORD5;
    #endif
};

void ComputeVertexLightColor(inout Interpolators interpolators)
{
    #if defined(VERTEXLIGHT_ON)
    interpolators.vertexLightColor = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb,
			unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, interpolators.worldPos, interpolators.normal
		);
    #endif
}

float3 CreateBinormal (float3 normal, float3 tangent, float binormalSign)
{
    return cross(normal,tangent.xyz)*(binormalSign*unity_WorldTransformParams.w);
}

Interpolators VertexProgram(VertexData vertexData)
{
    Interpolators interpolators;
    interpolators.position = UnityObjectToClipPos(vertexData.position);
    interpolators.worldPos = mul(unity_ObjectToWorld,vertexData.position);
    interpolators.uv.xy = TRANSFORM_TEX(vertexData.uv, _AlbedoTex);
    interpolators.uv.zw = TRANSFORM_TEX(vertexData.uv, _DetailTex);
    interpolators.normal = UnityObjectToWorldNormal(vertexData.normal); // normalize(mul(transpose((float3x3)unity_WorldToObject),vertexData.normal));
    
    #if defined(BINORMAL_PER_FRAGMENT)
    interpolators.tangent = float4(UnityObjectToWorldDir(vertexData.tangent.xyz), vertexData.tangent.w);
    #else
    interpolators.tangent = UnityObjectToWorldDir(vertexData.tangent.xyz);
    interpolators.binormal = CreateBinormal(interpolators.normal,interpolators.tangent,vertexData.tangent.w);
    #endif    
    
    ComputeVertexLightColor(interpolators);
    return interpolators;
}

UnityLight CreateLight(Interpolators interpolators)
{
    UnityLight light;
    
    #if defined(POINT) || defined(SPOT) || defined(POINT_COOKIE)
        light.dir = normalize(_WorldSpaceLightPos0.xyz - interpolators.worldPos);
    #else 
        light.dir = _WorldSpaceLightPos0.xyz;
    #endif
    
    UNITY_LIGHT_ATTENUATION(attenuation,0,interpolators.worldPos)
    light.color = _LightColor0.rgb * attenuation;  
    light.ndotl = DotClamped(interpolators.normal, light.dir);
    return light;
}

UnityIndirect CreateIndirectLight(Interpolators interpolators)
{
    UnityIndirect indirectLight;
    indirectLight.diffuse = 0;
    indirectLight.specular = 0;
    
	#if defined(VERTEXLIGHT_ON)
		indirectLight.diffuse = interpolators.vertexLightColor;
	#endif

	#if defined(FORWARD_BASE_PASS)
		indirectLight.diffuse += max(0, ShadeSH9(float4(interpolators.normal, 1)));
	#endif
	
    return indirectLight;
}

void InitializeFragmentNormal(inout Interpolators interpolators)
{   
     //  Bump mapping
     //  float2 delta1 = float2(_HeightMap_TexelSize.x * 0.5f,0);
     //  float2 delta2 = float2(0,_HeightMap_TexelSize.y * 0.5f);
     //  float derivative1 = tex2D(_HeightMap,interpolators.uv + delta1) - tex2D(_HeightMap,interpolators.uv - delta1);
     //  float derivative2 = tex2D(_HeightMap,interpolators.uv + delta2) - tex2D(_HeightMap,interpolators.uv - delta2);
     //   
     //  interpolators.normal = float3 (-derivative1,1,-derivative2);
     //  interpolators.normal = normalize(interpolators.normal);
     
     // Normal mapping with DXT5nm compresion - default by unity. 
     // This compresion stores only x and y of vector.
     //interpolators.normal.xy = tex2D(_NormalMap, interpolators.uv).wy * 2 - 1 * _BumpScale;
     //interpolators.normal.z = sqrt(1-saturate(dot(interpolators.normal.xy,interpolators.normal.xy)));
     //interpolators.normal.xy = normalize(interpolators.normal.xzy);
     
     //Normal mapping with unity prepared function
     float3 mainNormal = UnpackScaleNormal(tex2D(_NormalMap,interpolators.uv.xy), _BumpScale);
     float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, interpolators.uv.zw), _DetailBumpScale);
     
     float3 worldSpaceNormal = BlendNormals(mainNormal,detailNormal);
     
     #if defined(BINORMAL_PER_FRAGMENT)
     float3 binormal = CreateBinormal(interpolators.normal,interpolators.tangent.xyz, interpolators.tangent.w);
     #else
     float3 binormal = interpolators.binormal;
     #endif
     
     interpolators.normal = normalize(worldSpaceNormal.x * interpolators.tangent +
                                         worldSpaceNormal.y * binormal + 
                                         worldSpaceNormal.z * interpolators.normal); 
}

float4 FragmentProgram(Interpolators interpolators) : SV_TARGET
{
    InitializeFragmentNormal(interpolators);
                        
    float3 viewDir = normalize(_WorldSpaceCameraPos - interpolators.worldPos);
    
    float oneMinusReflectivity;
    float3 speculatTint;
    float3 albedoTexture = tex2D(_AlbedoTex,interpolators.uv.xy) * _Tint * tex2D(_DetailTex, interpolators.uv.zw) * unity_ColorSpaceDouble;
       
    albedoTexture = DiffuseAndSpecularFromMetallic(albedoTexture,_Metallic,speculatTint,oneMinusReflectivity); 
    
    return UNITY_BRDF_PBS(albedoTexture, speculatTint,
                          oneMinusReflectivity, _Smoothness,
                          interpolators.normal, viewDir,
                          CreateLight(interpolators), CreateIndirectLight(interpolators));
}

#endif