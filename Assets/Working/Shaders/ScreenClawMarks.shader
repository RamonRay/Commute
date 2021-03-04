Shader "MyShaders/ScreenClawMarks"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MulColor("Multiply Color", Color) = (1,1,1,1)
		_AddColor("Added Color", Color) = (0,0,0,1)
		_ClawTexture("Claw Texture", 2D) = "white" {}
		_Threshod("Claw showup threshod", Range(-1, 1)) = 0

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
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _ClawTexture;
			float4 _ClawTexture_ST;
			fixed4 _MulColor;
			fixed4 _AddColor;
			fixed _Threshod;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.uv.xy;
				o.uv.zw = TRANSFORM_TEX(v.uv, _ClawTexture);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				fixed4 clawMarkCol = tex2D(_ClawTexture, i.uv.zw);
				fixed3 clawMarkColFinal = clawMarkCol.rgb * clawMarkCol.a * step(_Threshod, (i.uv.w - i.uv.z));
				fixed3 originColor = tex2D(_MainTex, i.uv.xy).rgb;
				return fixed4(saturate(originColor * _MulColor.rgb + _AddColor.rgb + clawMarkColFinal), 1);
			}
			ENDCG
		}
	}
}
