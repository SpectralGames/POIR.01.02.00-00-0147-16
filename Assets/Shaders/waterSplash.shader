Shader "Custom/WaterSplash" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TintColor("Tint Color (RGB), Alpha (A)" , Color) = (1,1,1,1) 
		//_Cube ("Reflection Cubemap", Cube) = "" {}
		//_RimColor("Rim Color" , Color) = (1,1,1,1) 
		_NormalMap ("Normalmap", 2D) = "bump" {}
		_WaterAlphaSplash ("Alpha", 2D) =  "white" {}
		_Specular ("Specular" , Color) = (1,1,1,1) 
		//_Reflectance ("Reflectance", Range(0,1)) = 0.3
		_WaterSpeed ("Water Speed", Range(-1,1)) = 0.3
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular exclude_path:prepass alpha:fade interpolateview noforwardadd

		//inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten) //(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
		//{
		//	fixed diff = max (0, dot (s.Normal, lightDir));
		//	fixed nh = max (0, dot (s.Normal, halfDir));
		//	fixed spec = pow (nh, s.Specular*128) * s.Gloss;
			
		//	fixed4 c;
		//	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
		//	UNITY_OPAQUE_ALPHA(c.a);
		//	return c;
		//}

		sampler2D _MainTex, _NormalMap, _WaterAlphaSplash;
		//samplerCUBE _Cube;
		

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			//float3 worldNormal;
			//INTERNAL_DATA
		};

		half _Reflectance, _WaterSpeed;//
		fixed4 _Specular, _TintColor;
		//float4 _RimColor;

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			// Albedo comes from a texture tinted by color
			half y = fmod(IN.uv_MainTex.y + _Time[1]*_WaterSpeed, 1.0);
			fixed4 c = tex2D (_MainTex, float2(IN.uv_MainTex.x, y));
			//float3 worldRefl = WorldReflectionVector (IN, o.Normal);
			//fixed4 reflcol = texCUBE (_Cube, IN.viewDir);
			
			o.Albedo = c.rgb * _TintColor.rgb;// + reflcol * _Reflectance;
			o.Smoothness = _Specular.a;
			o.Specular = _Specular.rgb;
			//o.Occlusion = 0.4;
			o.Normal = UnpackNormal( tex2D (_NormalMap, float2(IN.uv_MainTex.x, y))) * UnpackNormal(tex2D(_NormalMap, float2(IN.uv_MainTex.x + _SinTime[0], IN.uv_MainTex.y+y*0.3)));

			fixed4 waterAlpha =  tex2D(_WaterAlphaSplash, IN.uv_MainTex);
			o.Alpha =waterAlpha.rgb; //normalMapColor.a; //c.a * sin(IN.uv_MainTex.x * 1.9);
            //half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
            //o.Emission = reflcol * _Reflectance; //_RimColor.rgb * pow(rim, 2f) + e.rgb*_Color.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
