Shader "MyShaders/DeferredAdsShader"
{
    Properties
    {
        _Color("Tint Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Diffuse("Diffuse", Color) = (1,1,1,1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _Gloss ("Gloss", Range(8.0, 256)) = 20
        _Mixer("Mixer between texture color and lighting color", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" /*"LightMode" = "Deferred"*/}
        LOD 100

        //ZTest Always
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma fragment MyFragProgram
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            struct deferredFragOutput{
		        float4 gBuffer0 : SV_TARGET0;
		        float4 gBuffer1 : SV_TARGET1;
		        float4 gBuffer2 : SV_TARGET2;
		        float4 gBuffer3 : SV_TARGET3;
	        };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            fixed4 _Diffuse;
            fixed4 _Specular;
	        float _Gloss;
            fixed _Mixer;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                //UNITY_TRANSFER_FOG(o,o.vertex);

                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 textureCol = tex2D(_MainTex, i.uv) * _Color;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Color.rgb;
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
		        fixed3 halfDir = normalize(worldLightDir + viewDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed4 lightingCol = fixed4(ambient + (diffuse + specular) * atten, 1);

                return textureCol * (1-_Mixer) + lightingCol * _Mixer;
            }

            ENDCG
        }

        Pass
        {
            Tags{"LightMode" = "ForwardAdd"}

            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma fragment MyFragProgram
            // make fog work
            //#pragma multi_compile_fog
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                
                float3 worldNormal : TEXCOORD1;
                SHADOW_COORDS(2)
                float3 worldPos : TEXCOORD2;
                
            };

            struct deferredFragOutput{
		        float4 gBuffer0 : SV_TARGET0;
		        float4 gBuffer1 : SV_TARGET1;
		        float4 gBuffer2 : SV_TARGET2;
		        float4 gBuffer3 : SV_TARGET3;
	        };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            fixed4 _Diffuse;
            fixed4 _Specular;
	        float _Gloss;
            fixed _Mixer;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                //UNITY_TRANSFER_FOG(o,o.vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 textureCol = tex2D(_MainTex, i.uv) * _Color;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Color.rgb;
                fixed3 diffuse = _LightColor0.rgb * _Color.rgb * max(0, dot(worldNormal, worldLightDir));
                fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
		        fixed3 halfDir = normalize(worldLightDir + viewDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(worldNormal, halfDir)), _Gloss);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                fixed4 lightingCol = fixed4((diffuse + specular) * atten, 1);

                return textureCol * (1-_Mixer) + lightingCol * _Mixer;
            }
            ENDCG
        }
    }
}
