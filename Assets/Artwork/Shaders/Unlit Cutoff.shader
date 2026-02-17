Shader "Virulent/Unlit Cutout" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader {
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
		LOD 250
			
			CGPROGRAM
			#pragma surface surf NotLit alpha alphatest:_Cutoff
			
			sampler2D _MainTex;
			fixed4 _Color;
			
			struct Input {
				float2 uv_MainTex;
			};
			
			half4 LightingNotLit (SurfaceOutput s, half3 lightDir, half atten) {
				half4 c;
				c.rgb = s.Albedo;
				c.a   = s.Alpha;
				
				return c;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
	}
	
	Fallback "Diffuse"
}
