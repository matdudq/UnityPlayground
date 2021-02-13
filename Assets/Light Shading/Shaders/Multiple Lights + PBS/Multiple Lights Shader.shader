Shader "Custom/Standard/Multiple Lights PBS"
{
    Properties
    {
        _AlbedoTex ("Albedo", 2D) = "white" {}
        _DetailTex ("Detail texture", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("Normal map",2D) = "bump"{}
        [NoScaleOffset] _DetailNormalMap ("Detail Normals", 2D) = "bump" {}
        _Tint ("Tint", Color) = (1,1,1,1)
         _BumpScale ("Bump scale",float) = 1
         _DetailBumpScale ("Detail Bump Scale", Float) = 1

        [Gamma] _Metallic ("Metalic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
    }

	CGINCLUDE

	#define BINORMAL_PER_FRAGMENT

	ENDCG

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
            
            #pragma multi_compile _ VERTEXLIGHT_ON
            #define FORWARD_BASE_PASS
            
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
                
            #include "Metalic PBS Shader.cginc"
                            
            ENDCG
        }
        
        Pass
        {
            Tags
            {
            "LightMode" = "ForwardAdd"
            }
        
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            
            #pragma target 3.0
            
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram
            
            #pragma multi_compile_fwdadd
            
            #include "Metalic PBS Shader.cginc"
                            
            ENDCG
        }
    }
}
