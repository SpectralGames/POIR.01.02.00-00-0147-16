Shader "Custom/TerrainShader" {
Properties{
		_MainTex ("MainTex", 2D) = "white" {}
		_SpecularTex ("Specular Tex", 2D) = "white"{}
		_NormalTex("Normal Map", 2D) = "white"{}
		_Mask("mask", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0,1)) = 1

		_NormalBaseTex("Base Normal Map", 2D) = "white"{}

		_DetailTex("DetailTex", 2D) = "white" {}
		_NormalDetailTex("Normal Detail Map", 2D) = "white"{}
		_SpecularDetailTex("Specular Detail Tex", 2D) = "white"{}
		_SmoothnessDetail("Smoothness Detail", Range(0,1)) = 1

		
		_SandTex ("Sand tex", 2D) = "white" {}
		_SandNormalTex ("Sand normal tex", 2D) = "white" {}
		_SandSpecular("SnadSpecular", Color) = (1,1,1,1)
		_SmoothnessSand("Smoothness Sand", Range(0,1)) = 1
		
	}
	SubShader{
		Tags{
			"RenderType" = "Opaque"  "Queue" = "Geometry"
		} 

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular
		sampler2D _MainTex;
		sampler2D _SpecularTex;
		sampler2D _NormalTex;
		sampler2D _Mask;

		sampler2D _NormalBaseTex;

		sampler2D _DetailTex;
		sampler2D _NormalDetailTex;
		sampler2D _SpecularDetailTex;

		sampler2D _SandTex;
		sampler2D _SandNormalTex;

		//half _Smoothness;
		half _Smoothness, _SmoothnessDetail, _SmoothnessSand;
		fixed4 _SandSpecular;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Mask;
			float2 uv_DetailTex;
			float2 uv_SandTex;
			float2 uv_NormalBaseTex;
		};

		void surf(Input IN, inout SurfaceOutputStandardSpecular o) {
			// main tex
			fixed4 mainTex =  tex2D(_MainTex, IN.uv_MainTex);
			fixed4 mainSpecularTex = tex2D(_SpecularTex, IN.uv_MainTex);
			fixed4 normalTex =  tex2D(_NormalTex, IN.uv_MainTex);
			fixed4 mask = tex2D(_Mask, IN.uv_Mask);
			// base
			fixed4 baseNormalTex = tex2D(_NormalBaseTex, IN.uv_NormalBaseTex);
			// detail
			fixed4 detailTex = tex2D(_DetailTex, IN.uv_DetailTex);
			fixed4 normalDetailTex = tex2D(_NormalDetailTex, IN.uv_DetailTex);
			fixed4 specularDetailTex = tex2D(_SpecularDetailTex, IN.uv_DetailTex);
			// sand
			fixed4 sandTex = tex2D(_SandTex, IN.uv_SandTex);
			fixed4 normalSandTex = tex2D(_SandNormalTex, IN.uv_SandTex);

			// calc albedo (main + detail + sand) detail mask -r , sand mask -g
			fixed4 c = lerp(mainTex, detailTex, mask.r);
			c = lerp(c, sandTex, mask.g);
			o.Albedo = c;

			// calc normal
			//fixed4 normal = lerp(normalTex, normalDetailTex, mask.r);
			//normal = lerp(normal, normalSandTex, mask.g);
			//o.Normal = UnpackNormal(normal);

			float3 normalBase = UnpackNormal(baseNormalTex);
			float3 normal = UnpackNormal(normalTex);
			float3 normalDetail = UnpackNormal(normalDetailTex);
			float3 normalSand = UnpackNormal(normalSandTex);

			float3 r = normalize(float3(normalBase.xy + normal.xy + (normalDetail.xy * mask.r) + (normalSand * mask.g), normalBase.z * normal.z * normalDetail.z * normalSand.z));

			o.Normal = r;
			// calc specular 
			fixed4 specularCount = lerp(mainSpecularTex, specularDetailTex, mask.r);
			specularCount = lerp(specularCount, _SandSpecular * mask.g , mask.g);
			o.Specular = specularCount; 

			// calc smoothness
			half smoothnessCount = mainSpecularTex.a * _Smoothness;
			smoothnessCount = lerp(smoothnessCount, specularDetailTex.a * _SmoothnessDetail , mask.r);
			smoothnessCount = lerp(smoothnessCount, _SandSpecular * _SmoothnessSand, mask.g);
			o.Smoothness = smoothnessCount;
			o.Occlusion = mask.b;
			
		}
		ENDCG
	}
	FallBack "Standard (Specular setup)"

}
