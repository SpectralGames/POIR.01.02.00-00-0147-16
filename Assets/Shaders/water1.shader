Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		//_DistortNormalTex ("Distort Normal (RGB)", 2D) = "white" {}
		_DistortScale ("Distort Scale", Range(0,4)) = 2
		_AdditionalLight("Additional Light", Range(0,4)) = 2
		_DistortStrength ("Distort Strength", Range(0,128)) = 10
		_Metallic ("Specular", Range(0,2)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_SubsurfaceScattering ("Subsurface Scattering", Range(0,4)) = 1.0
		_Alpha ("Opacity", Range(0,1)) = 0.5
		_WaterSpeed ("Water Speed", Range(-1,1)) = 0.3
		_WaveHeight ("Wave Height", Range(0,3)) = 1
		_InvFade ("Soft Factor", Range(0.01,10.0)) = 1.0
		_BounceHeight("Bounce Height", Range(0,3)) = 2
	}
	SubShader {
	
		GrabPass { "_MyGrabTexture" }
		
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
		LOD 200
		//Cull Off
		//Blend SrcAlpha OneMinusSrcAlpha
		//ColorMask RGB
		ZWrite Off
		
		CGPROGRAM
		
		#pragma surface surf WrapStandard fullforwardshadows vertex:vert alpha:fade
		#pragma multi_compile_particles
		//#pragma multi_compile_fog
	    #pragma target 3.0
		#include "UnityPBSLighting.cginc"

		half _SubsurfaceScattering;
		
		inline half4 LightingWrapStandard_Deferred (SurfaceOutputStandard s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
		{
			//half NdotL = dot (s.Normal, viewDir);
			//half diff = NdotL * 0.25 + 0.9;
			//half4 c;
			
			//c.rgb = s.Albedo * _LightColor0.rgb * (diff * (atten));// * _LightColor0.rgb * (diff * atten);
			//c.a = s.Alpha;
			//return c;
		
		
			s.Normal = normalize(s.Normal);
			half oneMinusReflectivity;
			half3 specColor;
			s.Albedo = DiffuseAndSpecularFromMetallic (s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

			
			 half diff = dot(s.Normal, gi.light.dir) * 0.15 + 0.85; //s.Normal  viewDir - gi.light.dir
			 //half dotNV = max(0, dot(s.Normal, viewDir) );
			 
			 const half4 c0 = { -1, -0.0275, -0.572, 0.022 };
			 const half4 c1 = { 1, 0.0425, 1.04, -0.04 };
		 	 half4 r = (1-s.Smoothness) * c0 + c1;
			 half a004 = min( r.x * r.x, exp2( -9.28 * diff ) ) * r.x + r.y;
			 half2 AB = half2( -1.04, 1.04 ) * a004 + r.zw;
			 half3 F_L = specColor * AB.x + AB.y;
			 
			 half3 transLightDir = gi.light.dir + s.Normal;
			 float transDot = pow ( saturate(dot ( viewDir, -transLightDir ) ), 0.95 );
			 half3 lightScattering = transDot * gi.light.color;
			
			
			
			half4 c;
			c.rgb = s.Albedo * (gi.indirect.diffuse*0.1 + gi.light.color * diff ) + (gi.indirect.specular * F_L) + lightScattering ;
			c += UNITY_BRDF_PBS (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect)*0.75;
			//c.rgb += UNITY_BRDF_GI (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
			c.a = s.Alpha;
			
			
			outDiffuseOcclusion = half4(c.rgb*0.5 + s.Albedo*0.5, s.Occlusion); //s.Albedo -> c.rgb
			outSpecSmoothness = half4(specColor, s.Smoothness);
			outNormal = half4(s.Normal * 0.5 + 0.5, 1); //half4(normalize(gi.light.dir * 0.5 + s.Normal * 0.1 + 0.4), 1); //half4(s.Normal * 0.5 + 0.5, 1)
			
			
			half4 emission = half4(c.rgb, 1);
			return emission;
		}
		
		
		inline half4 LightingWrapStandard (SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
		{
			 s.Normal = normalize(s.Normal);
	 
			 half oneMinusReflectivity;
			 half3 specColor;
			 s.Albedo = DiffuseAndSpecularFromMetallic (s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);
	 
			 // shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
			 // this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
			 //half outputAlpha;
			 //s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);


			 half diff = dot(s.Normal, viewDir) * 0.2 + 0.8; //s.Normal
			 //half dotNV = max(0, dot(s.Normal, viewDir) );
			 
			 const half4 c0 = { -1, -0.0275, -0.572, 0.022 };
			 const half4 c1 = { 1, 0.0425, 1.04, -0.04 };
		 	 half4 r = (1-s.Smoothness) * c0 + c1;
			 half a004 = min( r.x * r.x, exp2( -9.28 * diff ) ) * r.x + r.y;
			 half2 AB = half2( -1.04, 1.04 ) * a004 + r.zw;
			 half3 F_L = specColor * AB.x + AB.y;
			 
			 half3 halfDir = Unity_SafeNormalize (gi.light.dir + viewDir);
			 half lh = DotClamped (gi.light.dir, halfDir);
			 half realSmoothness = s.Smoothness*s.Smoothness;
			 half specularTerm = saturate(pow( dot(s.Normal, halfDir), 300) * realSmoothness) * 55 * (1-diff); //viewDir
			 
			 
			 half3 transLightDir = normalize(-viewDir + s.Normal);// gi.light.dir + s.Normal;
			 float transDot = pow ( saturate(dot ( viewDir, -transLightDir ) ) * 1.5, 1.8 ) * 0.8 + 0.2;
			 half3 lightScattering = transDot * specColor * _SubsurfaceScattering * gi.light.color;
			 
			 
			 half4 c;
			 //c = UNITY_BRDF_PBS (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
			 //c.rgb += lightScattering;
			 c.rgb = s.Albedo * (gi.indirect.diffuse*0.5 + gi.light.color * diff ) + (gi.indirect.specular * F_L) + lightScattering + specularTerm * specColor * FresnelTerm (specColor, lh); 

			 //c.rgb += UNITY_BRDF_GI (s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
			 c.a = s.Alpha;
			 return c;
		}
	

		inline void LightingWrapStandard_GI (SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			gi = UnityGlobalIllumination (data, s.Occlusion, s.Smoothness, s.Normal); // s.Normal  -> gi.light.dir
		}
		
		

		sampler2D _MainTex, _NormalTex;//, _DistortNormalTex;
		sampler2D _MyGrabTexture;
		float4 _MyGrabTexture_TexelSize;
		
		struct Input {
			float2 uv_MainTex;
			fixed4 grabUV;
			float3 worldPos;
			//float4 vertexPosition;
			//#ifdef SOFTPARTICLES_ON
            float4 projPos;
           // #endif
		};

		half _Glossiness, _DistortStrength, _WaterSpeed, _WaveHeight, _DistortScale, _BounceHeight, _AdditionalLight;
		half _Metallic, _InvFade;
		half _Alpha;
		fixed4 _Color;

		UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			v.vertex.xyz += v.normal * _BounceHeight * (sin(v.vertex.x + _Time[2])*0.45 * sin(v.vertex.z + _Time[0]*0.75)*0.2 - v.normal * 0.1);
			o.grabUV = ComputeGrabScreenPos(UnityObjectToClipPos(v.vertex));
			//o.vertexPosition = v.vertex;
			//#ifdef SOFTPARTICLES_ON
             o.projPos = ComputeScreenPos (UnityObjectToClipPos(v.vertex));
			 COMPUTE_EYEDEPTH(o.projPos.z);
           // #endif
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//Refrakcja
			float4 UVGrab = IN.grabUV;
			#if UNITY_SINGLE_PASS_STEREO
			UVGrab.xy = TransformStereoScreenSpaceTex(UVGrab.xy, UVGrab.w);
			#endif
			
			float y = fmod(IN.uv_MainTex.y + _Time[1]*_WaterSpeed, 1000.0);

			// calculate perturbed coordinates
			float2 offset = UnpackNormal(tex2D( _NormalTex, IN.worldPos.xz*_DistortScale + y )).rg * _DistortStrength * _MyGrabTexture_TexelSize.xy;
			#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE //to handle recent standard asset package on older version of unity (before 5.5)
				UVGrab.xy = offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(UVGrab.z) + UVGrab.xy;
			#else
				UVGrab.xy = offset * UVGrab.z + UVGrab.xy;
			#endif
			
			fixed4 refractionColor = tex2Dproj(_MyGrabTexture, UNITY_PROJ_COORD(UVGrab));
		
			// Albedo comes from a texture tinted by color
			
			fixed4 c = tex2D (_MainTex, float2(IN.uv_MainTex.x, y));
			
			float fade = 1.0;
			//#ifdef SOFTPARTICLES_ON
               float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.projPos)));
               float partZ = IN.projPos.z;
               fade = saturate (_InvFade * (sceneZ-partZ));
            //#endif
			
			o.Albedo = lerp(abs(sin(IN.worldPos.x + IN.worldPos.z + _Time[3]*_WaterSpeed) + sin(offset.x*3)*0.8)*1.2,   c.rgb + _Color.rgb * _AdditionalLight, fade-0.1)   *   lerp(refractionColor.rgb, float3(1,1,1), _Alpha) ;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Normal = UnpackScaleNormal(tex2D(_NormalTex, float2(IN.uv_MainTex.x, y)), _WaveHeight) * UnpackScaleNormal(tex2D(_NormalTex, float2(IN.uv_MainTex.x + _SinTime[1]*0.4, IN.uv_MainTex.y+y*0.2)), _WaveHeight*0.75);
			o.Alpha = saturate(fade*2.2);
		}
		ENDCG
	}
	Fallback "Standard (Specular setup)"
}
