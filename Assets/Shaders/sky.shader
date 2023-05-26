// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Sky" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_DayTex ("Day Texture (RGB)", 2D) = "white" {}
		_NightTex ("Night Texture (RGB)", 2D) = "white" {}
		_CloudsTex ("Clouds Texture (RGB)", 2D) = "white" {}
		[HDR]
		_SunColor ("Sun Color", Color) = (1,1,1,1)
		[HDR]
		_SunGlowColor ("Sun Glow Color", Color) = (1,1,1,1)
		[HDR]
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_DayNightBlend ("Day-Night Blend", Range(0,1)) = 0.5
		_SunForwardVector ("Sun Forward Vector", Vector) = (0,1,0,1)
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		//ZWrite Off

		CGPROGRAM
		// Physically based NoLighting lighting model, and enable shadows on all light types
		#pragma surface surf Lambert vertex:vert nodynlightmap exclude_path:prepass noshadow//fullforwardshadows

		inline fixed4 LightingNoLighting (SurfaceOutput s, fixed3 halfDir, fixed atten)
		{
			//fixed diff = max (0, dot (s.Normal, lightDir));
			//fixed nh = max (0, dot (s.Normal, halfDir));
			//fixed spec = pow (nh, s.Specular*128) * s.Gloss;
	
			fixed4 c;
			c.rgb = s.Albedo;// * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			UNITY_OPAQUE_ALPHA(c.a);
			return c;
		}
		
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _DayTex, _NightTex, _CloudsTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldNormal;
			//float3 texcoord : TEXCOORD0;
		};

		half _DayNightBlend;
		fixed4 _Color, _EmissionColor, _SunColor, _SunForwardVector, _SunGlowColor;

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.worldNormal = mul(unity_ObjectToWorld, v.normal);
			//o.texcoord = v.texcoord;
           // #endif
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			// Albedo comes from a texture tinted by color
			//fixed2 uvOffset = fixed2(IN.uv_MainTex.x + _Time[0]*0.04, IN.uv_MainTex.y);
			fixed4 day = tex2D (_DayTex, IN.uv_MainTex);
			fixed4 night = tex2D (_NightTex, IN.uv_MainTex);
			fixed3 clouds = tex2D (_CloudsTex, float2(IN.uv_MainTex.x + sin(_Time[0]*0.3)*0.1 + _Time[0]*0.1, IN.uv_MainTex.y + sin(_Time[0])*0.04 + 0.1)).rgb * 1.3;
			fixed3 clouds2 = tex2D (_CloudsTex, float2(IN.uv_MainTex.x - sin(_Time[0]*0.15)*0.1 - _Time[0]*0.06, IN.uv_MainTex.y - sin(_Time[0])*0.04+0.02)).rgb * 1.5;
			fixed3 finalColor = lerp(day.rgb, night.rgb, _DayNightBlend) * _Color.rgb;

			//fixed3 sunColor = _SunColor.rgb * smoothstep(0.0, 2.0,  pow(  saturate(dot(IN.worldNormal, normalize(_SunForwardVector.xyz))),  _SunColor.a*500)   );
			float normalToSunLength = saturate(length(IN.worldNormal - normalize(_SunForwardVector.xyz)));
			fixed3 sunGlowColor = _SunGlowColor.rgb * smoothstep(0.0, 1.0,  pow(  1.0 - normalToSunLength,  _SunGlowColor.a*5)   );
			fixed3 sunColor = _SunColor.rgb * smoothstep(0.0, 2.0,  pow(  1.0 - normalToSunLength,  _SunColor.a*200)   );
			o.Albedo = finalColor + clouds + clouds2;
			o.Emission = finalColor * _EmissionColor + clouds*0.1 + sunColor*saturate(1-_DayNightBlend*3) + sunGlowColor*saturate(1-_DayNightBlend*3)*_DayNightBlend;

			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
