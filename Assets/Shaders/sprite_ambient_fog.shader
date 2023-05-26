Shader "Custom/SpriteAmbientFog"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="AlphaTest" 
			"IgnoreProjector"="True" 
			"RenderType"="TransparentCutout" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		//ZWrite Off
		//Blend One OneMinusSrcAlpha

		Pass
		{
		Tags {"LightMode"="ForwardBase"}
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma target 2.0
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			uniform fixed _Cutoff;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				
				half3 worldNormal = UnityObjectToWorldNormal(IN.normal);
                //half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                //fixed4 diff = nl * _LightColor0;

                // in addition to the diffuse lighting from the main light,
                // add illumination from ambient or light probes
                // ShadeSH9 function from UnityCG.cginc evaluates it,
                // using world space normal
				fixed4 diff = fixed4(0,0,0,0);
                diff.rgb += ShadeSH9(half4(worldNormal,1));
				diff.a = 1;
				
				OUT.color = diff * _Color; //+ IN.color * _Color;
				UNITY_TRANSFER_FOG(OUT,OUT.vertex);

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
         
				UNITY_APPLY_FOG(IN.fogCoord, c);
				
				clip( c.a - _Cutoff ); //AlphaTest zamiast alpha blending
				//c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
