﻿Shader "Snowbound/TransitionUI"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

		_MaskTexture("Mask Texture", 2D) = "white" {}
		_Elapsed("Elapsed Seconds", Range(0, 1)) = 0.0
		_SoftFudge("Percent Softness", Range(0, 1)) = 0.1
		_Invert("Invert", Range(0, 1)) = 0.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};

			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

		#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
		#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _MaskTexture;
			float _Elapsed;
			float _SoftFudge;
			int _Invert;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

		#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
		#endif

				float maskValue = tex2D(_MaskTexture, IN.texcoord).a;

				// prevent rounding issues hack
				maskValue *= (1.0 - 1.0 / 255.0);

				// the leading edge takes (1.0-softFudge) to finish
				float adjustedElapsed = 1.0 - (_Elapsed * (1.0 + _SoftFudge));
				float weightLow = adjustedElapsed - maskValue;
				float weightHigh = (adjustedElapsed + _SoftFudge) - maskValue;
				float weight = ((weightLow + weightHigh) / 2.0) / _SoftFudge;
				weight = clamp(weight, 0.0, 1.0);
				if (_Invert) {
					weight = (1.0f - weight);
				}

				color.a = lerp(0.0f, color.a, weight);

				return color;
			}
			ENDCG
		}
	}
}
