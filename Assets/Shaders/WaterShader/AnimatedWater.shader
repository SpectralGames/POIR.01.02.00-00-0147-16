// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AnimatedWater" {
	Properties {

		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		_Gloss("Gloss", Range(0,1)) = 1
		_Color("Color", Color) = (1,1,1,1)
		_Specular("Specular", Color) = (1,1,1,1)
		_WaterSpeed("Water Speed", Range(-5,5)) = 0.3
			//_Alpha ("Alpha", Range(0,1)) = 1
	}
		SubShader{
			//Tags { "RenderType"="Opaque" }
			Tags{ "RenderType" = "Transparent" "Queue" = "Geometry" } //musi sie renderowac PRZED UVFilterShader
			LOD 200


			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf StandardSpecular alpha:fade

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex, _NormalTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalTex;
		};

		uniform float _AR_AmbientIntensity = 1.0;
		uniform float4 _AR_AmbientColorTemperature = float4(1.0, 1.0, 1.0, 1.0);
		//half _Alpha,
		half _Gloss, _WaterSpeed;
		fixed4 _Color, _Specular;



		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {

			// Albedo comes from a texture tinted by color
			half y = fmod(IN.uv_NormalTex.y + _Time[1] * _WaterSpeed, 1.0);

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color; //* _AR_AmbientIntensity * _AR_AmbientColorTemperature;
			o.Albedo = c.rgb;
			//o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex *_SinTime[0]));
		//	o.Normal = UnpackNormal(tex2D(_NormalTex, float2(IN.uv_MainTex.x, y))) * UnpackNormal(tex2D(_NormalTex, float2(IN.uv_MainTex.x , IN.uv_MainTex.y + y * 0.3)));
			

		//	fixed4 normalTex = tex2D(_NormalTex, float2(IN.uv_NormalTex.x + ( 0.07 * _SinTime[0]), IN.uv_NormalTex.y - (0.07 * _SinTime[0])));
		//	fixed4 normalTex2 = tex2D(_NormalTex, float2(IN.uv_NormalTex.x - (0.07 * _SinTime[0]), IN.uv_NormalTex.y + (0.07 * _SinTime[0])));
		//	o.Normal = UnpackNormal(normalTex * normalTex2 * 0.4);

			fixed4 normalTex = tex2D(_NormalTex,  float2(IN.uv_NormalTex.x + 0.07, IN.uv_NormalTex.y - (_Time[0] * 0.20 )));
			fixed4 normalTex2 = tex2D(_NormalTex, float2(IN.uv_NormalTex.x - 0.2, IN.uv_NormalTex.y + (_Time[0] * 0.20)));

		//	fixed4 normalTex2 = tex2D(_NormalTex, float2(IN.uv_NormalTex.x - (0.07 * _SinTime[0]), IN.uv_NormalTex.y + (0.07 * _SinTime[0])));

			o.Normal = UnpackNormal(normalTex * normalTex2 * 05) ;
			
		

			o.Alpha = c.a;
			o.Specular = _Specular; //* _AR_AmbientIntensity;
			o.Smoothness = _Gloss;//specularTex.a * _Gloss;

		}
		ENDCG
	}
	FallBack "Standard (Specular setup)"
}
