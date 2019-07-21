Shader "Custom/DistortionFlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

	    //流动方向贴图，储存贴图向量方向
		[NoScaleOffset] _FlowMap("Flow (RG，A noise)", 2D) = "black" {}
		[NoScaleOffset] _DerivHeightMap("Deriv (AG) Height (B)", 2D) = "black" {}
		_Color("Color Tint", Color) = (1, 1, 1, 1)

		_Specular("Specular", Color) = (1, 1, 1, 1)
		_Gloss("Gloss", Range(0.1, 10)) = 0.8

		_UJump("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_Tiling("Tiling", Float) = 1
		_Speed("Speed", Float) = 1
		_FlowStrength("Flow Strength", Float) = 1
		_FlowOffset("Flow Offset", Float) = 0
		_HeightScale("Height Scale, Constant", Float) = 0.25
		_HeightScaleModulated("Height Scale, Modulated", Float) = 0.75
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
			#include "Lighting.cginc"
            #include "Flow.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				// float2 flow_uv : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 world_pos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _FlowMap, _DerivHeightMap;
			float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset, _HeightScale, _HeightScaleModulated;
			fixed4 _Specular;
			float _Gloss;
			fixed4 _Color;


			float3 UnpackDerivativeHeight(float4 textureData) {
				float3 dh = textureData.agb;
				dh.xy = dh.xy * 2 - 1;
				return dh;
			}


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.world_pos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				//顶点与片元相同，同一平面上  (1-t)(b1+Time)+t(b2+Time) = b3 +Time

				//mapping到-1到1
				float3 flow = tex2D(_FlowMap, i.uv).rgb;
			    flow.xy = flow.xy * 2 - 1;
			    flow *= _FlowStrength;

				float time = _Time.y * _Speed + tex2D(_FlowMap,i.uv).a;

				float2 jump = float2(_UJump, _VJump);

				float3 uvwA = flowUV(i.uv, flow.xy, jump, _FlowOffset,_Tiling, time,false);
				float3 uvwB = flowUV(i.uv, flow.xy, jump, _FlowOffset,_Tiling, time, true);

				float finalHeightScale = flow.z * _HeightScaleModulated + _HeightScale;

				float3 dhA = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * (uvwA.z * finalHeightScale);
				float3 dhB = UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * (uvwB.z * finalHeightScale);

				fixed3 Normal = normalize(float3(-(dhA.xy + dhB.xy), 1));


				fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
				fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

				fixed4 c = (texA + texB) * _Color;

				fixed4 albedo = c;

				fixed3 lightdirnormalize = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo.rbg;

				fixed3 diffuse = _LightColor0.rgb * albedo.rbg * max(0, dot(lightdirnormalize, Normal));

				fixed3 halfvec = normalize(normalize(_WorldSpaceCameraPos.xyz - i.world_pos) + lightdirnormalize);

				fixed3 specular = _LightColor0.rgb * albedo.rbg * _Specular.rgb * pow(max(0, dot(halfvec, Normal)), _Gloss);

				return fixed4(ambient + diffuse + specular, 1.0);

            }
            ENDCG
        }
    }
}
