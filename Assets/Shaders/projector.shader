Shader "Custom/Projector/Additive" 
{
	Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ShadowTex ("Cookie", 2D) = "gray" {}
	_FalloffTex ("FallOff", 2D) = "white" {}
	_Power ("Power", Float) = 1
	}
	
	Subshader 
	{
	Tags {"Queue"="Transparent"}
	Pass {
		ZWrite Off
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		Offset -1, -1
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fog
		#include "UnityCG.cginc"
		
		struct v2f {
		float4 uvShadow : TEXCOORD0;
		float4 uvFalloff : TEXCOORD1;
		UNITY_FOG_COORDS(2)
		float4 pos : SV_POSITION;
		};
		
		float4x4 unity_Projector;
		float4x4 unity_ProjectorClip;
		v2f vert (float4 vertex : POSITION)
		{
			v2f o;
			o.pos = UnityObjectToClipPos (vertex);
			o.uvShadow = mul (unity_Projector, vertex);
			o.uvFalloff = mul (unity_ProjectorClip, vertex);
			UNITY_TRANSFER_FOG(o,o.pos);
			return o;
		}
		sampler2D _ShadowTex;
		sampler2D _FalloffTex;
		 
		float _Power;
		fixed4 _Color;
		fixed4 frag (v2f i) : SV_Target
		{
			 if (i.uvShadow.w > 0.0) // in front of projector?
             {
                fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				texS.rgb *= _Color.rgb;
				//texS.a = 1.0-texS.a;
		
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 res = texS * saturate(texF.a * 2);

				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
				 
				return res * _Power;
             }
             else // behind projector
             {
               return fixed4(0.0, 0.0, 0.0, 0.0);
             }
			
		}
		ENDCG
		}
	}
}
