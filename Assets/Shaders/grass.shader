Shader "Custom/Grass" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecularColor ("Specular Color", Color) = (1,1,1,1)
		_SpecularTex ("Specular (RGBA)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		_WindTex ("Wind Strenght Texture (RGB)", 2D) = "white" {}
		//_Metallic ("Specular", Range(0,1)) = 0.0
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
		_SubsurfaceScattering ("Subsurface Scattering", Range(0,8)) = 1.0
		_NormalScale ("Normal Scale", Range(0,2)) = 1.0
	}
	
	SubShader 
	{
		Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" "IgnoreProjector"="False"}
		//LOD 200
		Cull Off
		//Blend SrcAlpha OneMinusSrcAlpha
		//ColorMask RGB
		ZWrite On
		
		CGPROGRAM
		
		#pragma surface surf WrapStandardSpecular vertex:vert alphatest:_Cutoff  //nodirlightmap
		//#pragma multi_compile_fog
	    #pragma target 3.0
		#pragma multi_compile_instancing
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
			 
			 float3 H = normalize(L + N * 0.82);
			 float I = pow(saturate(dot(V, -H)), 0.8) * _SubsurfaceScattering;
			 
			 // Final add
			 pbr.rgb = pbr.rgb + I * s.Albedo;//gi.light.color * I * s.Albedo;
			 return pbr;
		}
	
		inline fixed4 LightingWrapStandardSpecular(SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
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
			 pbr.rgb = pbr.rgb + I * gi.light.color * I * s.Albedo;
			 return pbr;
		}
		
		
		
		inline void LightingWrapStandardSpecular_GI ( SurfaceOutputStandardSpecular s, UnityGIInput data, inout UnityGI gi)
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, 1, s.Normal);
			#else
				Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.Smoothness, data.worldViewDir, s.Normal, s.Specular);
				gi = UnityGlobalIllumination(data, 1, s.Normal, g);
			#endif
		}
		
		

		sampler2D _MainTex, _NormalTex, _SpecularTex, _WindTex;
		
		struct Input 
		{
			float2 uv_MainTex;
		};

		//half _Glossiness;
		//half _Metallic;
		half _NormalScale;
		fixed4 _Color, _SpecularColor;

		
		void vert(inout appdata_full v, out Input o)
		{	
			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
            fixed perlinTextureFactor = 0.8 * (tex2Dlod(_WindTex, float4(worldPos.x*0.01 + _Time[0]*0.45, worldPos.z*0.01, 0, 0) ).r - 0.5);
            half maskFactor = v.texcoord.y + 0.5; //v.vertex.y + 0.5; //tex2Dlod(_WindInfluenceTex, float4(v.texcoord.xy, 0, 0)).r;
			v.normal.xyz = mul(unity_WorldToObject, half3(0.0, 1.0, 0.0));
			
            v.vertex.xyz += mul(unity_WorldToObject, half4(0.5, 0.0, 0.5, 0.0)) * perlinTextureFactor * maskFactor; 
		}

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			// Albedo comes from a texture tinted by color
			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			//fixed4 s = tex2D (_SpecularTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb * _Color.rgb;
			o.Specular = _SpecularColor.rgb; //s.rgb;
			o.Smoothness = _SpecularColor.a; //s.a;
			//o.Occlusion = 2;
			o.Normal = UnpackScaleNormal(tex2D(_NormalTex, IN.uv_MainTex), _NormalScale);
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Transparent/Cutout/VertexLit"
}
