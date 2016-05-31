﻿Shader "Snowbound/TransitionShader"
{
	Properties
	{
		_MainTexture ("Main Texture", 2D) = "white" {}
		_MaskTexture ("Mask Texture", 2D) = "white" {}
		_Elapsed ("Elapsed Seconds", Range(0,1)) = 0.0
		_SoftFudge ("Percent Softness", Range(0, 1)) = 0.1
		_Invert ("Invert", Range(0, 1)) = 0.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTexture;
			sampler2D _MaskTexture;
			float _Elapsed;
			float _SoftFudge;
			int _Invert;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainColor = tex2D(_MainTexture, i.uv);
				float maskValue = tex2D(_MaskTexture, i.uv).a;

				// prevent rounding issues hack
				maskValue *= (1.0 - 1.0 / 255.0);

				// the leading edge takes (1.0-softFudge) to finish
				float adjustedElapsed = _Elapsed * (1.0 + _SoftFudge);
				float weightLow = adjustedElapsed - maskValue;
				float weightHigh = (adjustedElapsed + _SoftFudge) - maskValue;
				float weight = ((weightLow + weightHigh) / 2.0) / _SoftFudge;
				weight = clamp(weight, 0.0, 1.0);
				if (_Invert) {
					weight = (1.0f - weight);
				}

				mainColor.rgb = lerp(fixed4(0.0, 0.0, 0.0, 1.0), mainColor.rgb, 1.0-weight);

				return mainColor;
			}
			ENDCG
		}
	}
}
