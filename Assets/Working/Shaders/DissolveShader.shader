Shader "R1T7/DissolveEffect"
{
	Properties{
		_Diffuse("Diffuse", Color) = (1,1,1,1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
		_DetailColor("Detail Color", Color) = (0,0,0,0)
		_ColorAdjust("ColorAdjust", Range(0,5)) = 1
		_ExtraColor("Extra Color", Color) = (0,0,0,0)
		_ExtraColorAdjust("Extra Color Adjust", Range(0,2)) = 1
		_Gloss ("Gloss", Range(8.0, 256)) = 20
		_BumpScale("Bump Scale", Range(0.0, 5)) = 1.0
		_DissolveColor("Dissolve Color", Color) = (0,0,0,0)
		_DissolveEdgeColor("Dissolve Edge Color", Color) = (1,1,1,1)
		_MainTex("Base 2D", 2D) = "white"{}
		_EyeDetialTex("Eye Detail Texture", 2D) = "white"{}
		_BumpMap("Normal Map", 2D) = "white"{}
		_SmoothMap("Smooth Map", 2D) = "white"{}
		_DissolveMap("DissolveMap", 2D) = "white"{}
		_DissolveThreshold("DissolveThreshold", Range(0,1)) = 0
		_ColorFactor("ColorFactor", Range(0,1)) = 0.7
		_DissolveEdge("DissolveEdge", Range(0,1)) = 0.8
        _FlyUpVal("FlyUpValue", Range(0,1)) = 0
        _FlyThreshold("FlyThreshold", Range(0,1)) = 0
        _NormalUpRate("NormalUpRate", Range(0,1)) = 0.5
	}
	
	CGINCLUDE
	#include "Lighting.cginc"
	#pragma target 4.5
	//#pragma multi_compile_fwdbase
	//#define DEFERRED_PASS

	uniform fixed4 _Diffuse;
	uniform fixed4 _DissolveColor;
	uniform fixed4 _DissolveEdgeColor;
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	sampler2D _BumpMap;
	sampler2D _SmoothMap;
	sampler2D _EyeDetialTex;
    fixed4 _Specular;
	fixed4 _DetailColor;
	float _Gloss;
	float _BumpScale;
	uniform sampler2D _DissolveMap;
    float4 _DissolveMap_ST;
	uniform float _DissolveThreshold;
	uniform float _ColorFactor;
	uniform float _DissolveEdge;
    float _FlyUpVal;
    float _FlyThreshold;
    float _NormalUpRate;
	float _ColorAdjust;
	float _ExtraColorAdjust;
	fixed4 _ExtraColor;

	
	

	struct appdata 
	{
		float4 vertex : POSITION;
		float4 normal : NORMAL;
		float4 texcoord : TEXCOORD0;
		//UNITY_VERTEX_INPUT_INSTANCE_ID
		float4 tangent : TANGENT;
	};


	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		float4 TtoW0 : TEXCOORD1;  
		float4 TtoW1 : TEXCOORD2;  
		float4 TtoW2 : TEXCOORD3; 
		
		//UNITY_VERTEX_OUTPUT_STEREO
	};

	struct deferredFragOutput{
		float4 gBuffer0 : SV_TARGET0;
		float4 gBuffer1 : SV_TARGET1;
		float4 gBuffer2 : SV_TARGET2;
		float4 gBuffer3 : SV_TARGET3;
	};
	
	v2f vert(appdata v)
	{
		v2f o;

		//UNITY_SETUP_INSTANCE_ID(v); //Insert
    	//UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
    	//UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

        float3 upVec = float3(0,1,0);
        v.vertex.xyz += normalize(_NormalUpRate * upVec + (1-_NormalUpRate)*v.normal) * saturate(_DissolveThreshold-_FlyThreshold) * _FlyUpVal;

		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);

        o.uv.zw = TRANSFORM_TEX(v.texcoord, _DissolveMap);
		
		fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  
		fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

		o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
		o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
		o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

		return o;
	}
	

	deferredFragOutput MyFragProgram(v2f i){

		deferredFragOutput output;

		fixed4 dissolveValue = tex2D(_DissolveMap, i.uv.zw);

		if (dissolveValue.r < _DissolveThreshold)
		{
			discard;
		}

		float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
        
		//fixed3 worldNormal = normalize(i.worldNormal);
		fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));

		fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
		bump.xy *= _BumpScale;
		bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
		bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));

		fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Diffuse.rgb;
		half3 metallic = tex2D(_SmoothMap, i.uv.xy).rgb;
		half smoothness = tex2D(_SmoothMap, i.uv.xy).r;
		half eyeDetialVal = tex2D(_EyeDetialTex, i.uv.xy).r;
        fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
        fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(bump, worldLightDir));

		fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
		fixed3 halfDir = normalize(worldLightDir + viewDir);
		fixed3 specular = _LightColor0.rgb /** _Specular.rgb*/ * pow(max(0, dot(bump, halfDir)), _Gloss);

		fixed3 color = (ambient + diffuse) * _ColorAdjust /*+ specular*/ + _ExtraColor.rgb * _ExtraColorAdjust + eyeDetialVal * _DetailColor;
 
		float percentage = _DissolveThreshold / dissolveValue.r;

		float lerpEdge = sign(percentage /*- _ColorFactor*/ - _DissolveEdge);

		fixed3 edgeColor = lerp(_DissolveEdgeColor.rgb, _DissolveColor.rgb, saturate(lerpEdge));

		float lerpOut = sign(percentage - _ColorFactor);

		fixed3 colorOut = lerp(color, edgeColor, saturate(lerpOut));

		output.gBuffer0.rgb = albedo;
		output.gBuffer0.a = smoothness;
		//output.gBuffer1.rgb = _Specular.rgb;
		output.gBuffer1.rgb = metallic;
		output.gBuffer1.a = smoothness; // smoothness
		output.gBuffer2 = fixed4(bump, 1);
		output.gBuffer3 = fixed4(colorOut, 1);
		return output;
		//return fixed4(colorOut, 1);
	}
	ENDCG
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "Deferred" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			//#pragma fragment frag
			#pragma fragment MyFragProgram	
			ENDCG
		}
	}
	FallBack "Diffuse"
}