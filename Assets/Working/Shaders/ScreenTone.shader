﻿Shader "MyShaders/ScreenTone"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MulColor("Multiply Color", Color) = (1,1,1,1)
		_AddColor("Added Color", Color) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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

			sampler2D _MainTex;
			fixed4 _MulColor;
			fixed4 _AddColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 originColor = tex2D(_MainTex, i.uv).rgb;
				return fixed4(saturate(originColor * _MulColor.rgb + _AddColor.rgb), 1);
			}
			ENDCG
		}
	}
}
