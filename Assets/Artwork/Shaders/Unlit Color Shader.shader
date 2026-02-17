Shader "Virulent/Unlit Color" {
	Properties {
		_Color("Color (RGB)", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf NotLit

		half4 _Color;

		half4 LightingNotLit (SurfaceOutput s, half3 lightDir, half atten) {
			return _Color;
		}

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			//o.Albedo = _Color.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
