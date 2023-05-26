Shader "Custom/Flag" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecularTex ("Specular (RGBA)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		//_DistortNormalTex ("Distort Normal (RGB)", 2D) = "white" {}
		//_DistortScale ("Distort Scale", Range(0,4)) = 2
		//_DistortStrength ("Distort Strength", Range(0,128)) = 10
		//_Metallic ("Specular", Range(0,1)) = 0.0
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_SubsurfaceScattering ("Subsurface Scattering", Range(0,4)) = 1.0
	}
	
	SubShader 
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
		LOD 200
		Cull Off
		//Blend SrcAlpha OneMinusSrcAlpha
		//ColorMask RGB
		ZWrite On
		
		CGPROGRAM
		
		#pragma surface surf WrapStandardSpecular //fullforwardshadows
		//#pragma multi_compile_fog
	    #pragma target 3.0
		#include "UnityPBSLighting.cginc"

		half _SubsurfaceScattering;
		
		
		inline fixed4 LightingWrapStandardSpecular_Deferred(SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi, out half4 outGBuffer0, out half4 outGBuffer1, out half4 outGBuffer2)
		{
			 // Original colour
			 fixed4 pbr = LightingStandardSpecular_Deferred(s, viewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
			 
			 // --- Translucency ---
			 float3 L = gi.light.dir;
			 
			 float3 V = viewDir;
			 float3 N = s.Normal;
			 
			 float3 H = normalize(L + N * 0.2 - viewDir * 0.2);
			 float I = pow(saturate(dot(V, -H)), 1.05) * _SubsurfaceScattering;
			 
			 // Final add
			 pbr.rgb = pbr.rgb + I * s.Albedo; //gi.light.color * I * s.Albedo;
			 return pbr;
		}
		
		inline fixed4 LightingWrapStandardSpecular(SurfaceOutputStandardSpecular s, fixed3 viewDir, UnityGI gi)
		{
			 // Original colour
			 fixed4 pbr = LightingStandardSpecular(s, viewDir, gi);
			 
			 // --- Translucency ---
			 float3 L = gi.light.dir;
			 
			 float3 V = viewDir;
			 float3 N = s.Normal;
			 
			 float3 H = normalize(L + N * 0.6);
			 float I = pow(saturate(dot(V, -H)), 1.05) * _SubsurfaceScattering;
			 
			 // Final add
			 pbr.rgb = pbr.rgb + gi.light.color * I * s.Albedo;
			 return pbr;
		}
	
		
		
		
		inline void LightingWrapStandardSpecular_GI ( SurfaceOutputStandardSpecular s, UnityGIInput data, inout UnityGI gi)
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.Smoothness, data.worldViewDir, s.Normal, s.Specular);
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal, g);
			#endif
		}
		
		

		sampler2D _MainTex, _NormalTex, _SpecularTex;
		
		struct Input 
		{
			float2 uv_MainTex;
		};

		//half _Glossiness;
		//half _Metallic;
		fixed4 _Color;


		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			// Albedo comes from a texture tinted by color
			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 s = tex2D (_SpecularTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb * _Color.rgb;
			o.Specular = s.rgb;
			o.Smoothness = s.a;
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
			//o.Alpha = fade;
		}
		ENDCG
	}
	Fallback "Standard (Specular setup)"
}
