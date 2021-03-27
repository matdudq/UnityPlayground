Shader "Custom/Standard/Bllin Phong IBL"
{
    Properties
    {
        _AlbedoTex ("Albedo", 2D) = "white" {}
        [NoScaleOffset] _DiffuseHDR ("Diffuse HDR", 2D) = "white" {}
        _DiffuseIBLIntensity ("Diffuse IBL Intensity", Range(0,1)) = 0.5
        [NoScaleOffset] _SpecularHDR ("Specular HDR", 2D) = "white" {}
        _SpecularIBLIntensity ("Specular IBL Intensity", Range(0,1)) = 0.5
        _FresnelIntensity ("Fresnel Mask Intensity", Range(0,1)) = 0.5
        [NoScaleOffset] _NormalMap ("Normal map",2D) = "bump"{}
        
        Tint ("Tint", Color) = (1,1,1,1)
        _SpecularTint ("Specular Tint", Color) = (1,1,1,1)
        _Smoothness ("_Smoothness", Range(0,1)) = 0.5
    }

    SubShader
    {
        
        
        Pass
        {    
            Tags { "LightMode" = "ForwardBase"}
            CGPROGRAM     
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram      
            #define IS_IN_BASE_PASS
            #include "Blin Phong IBL Shader.cginc"  
            ENDCG
        }
        
        Pass
        {
            Tags { "LightMode" = "ForwardAdd"}
            Blend One One
            CGPROGRAM     
            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram   
            #pragma multi_compile_fwdadd   
            #include "Blin Phong IBL Shader.cginc"  
            ENDCG
        }
    }
}
