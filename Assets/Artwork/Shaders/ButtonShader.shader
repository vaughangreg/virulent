Shader "Button Shader" {
	Properties {
		_ButtonUp ("Button Up (RGB)", 2D) = "white" {}
		_ButtonDown ("Button Down (RGB)", 2D) = "white" {}
		_Overlay ("Overlay (RGBA)", 2D) = "white" {}
		_Image ("Image (RGBA)", 2D) = "white" {}
		_Cutoff ("Overlay alpha cutoff", Range(0.0, 1)) = 0.5
		_IsDown ("State (Up-Down)", Range(0.0, 1)) = 0
		_ImageShift("Image change when down", Range(0, 1)) = 0.25
		_Tint("Tint (RGB)", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf NotLit

		sampler2D _ButtonUp;
		sampler2D _ButtonDown;
		sampler2D _Overlay;
		sampler2D _Image;
		float     _Cutoff;
		float     _IsDown;
		float	  _ImageShift;
		half4	  _Tint;

		struct Input {
			float2 uv_ButtonUp;
		};
		
		half4 LightingNotLit(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a   = s.Alpha;
			
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			// Blend between the Up & Down textures.
			half4 texColor = tex2D(_ButtonDown, IN.uv_ButtonUp) * _IsDown + tex2D(_ButtonUp, IN.uv_ButtonUp) * (1.0f - _IsDown);
			
			// Blend the image into the texture
			half4 imageColor = tex2D(_Image, IN.uv_ButtonUp);
			//texColor = texColor + (imageColor * imageColor.a) - (_IsDown * _ImageShift * imageColor.a);
			texColor = (texColor * (1.0 - imageColor.a)) + (imageColor * imageColor.a);
			
			// Add the swipe
			imageColor = tex2D(_Overlay, IN.uv_ButtonUp);
			bool useOverlay = imageColor.a < _Cutoff;
			
			//texColor.rgb * imageColor.a + imageColor.rgb * (1.0f - imageColor.a)
			o.Albedo = (useOverlay ? (texColor.rgb * imageColor.a) : texColor.rgb) * _Tint.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
