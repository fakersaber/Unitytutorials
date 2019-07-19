Shader "Custom/DistortionFlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

	    //流动方向贴图，储存贴图向量方向
		[NoScaleOffset] _FlowMap("Flow (RG，A noise)", 2D) = "black" {}

		_UJump("U jump per phase", Range(-0.25, 0.25)) = 0.25
		_VJump("V jump per phase", Range(-0.25, 0.25)) = 0.25
		_Tiling("Tiling", Float) = 1
		_Speed("Speed", Float) = 1
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
            #include "Flow.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				//float2 flow_uv : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				//float2 flow_uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _FlowMap;
			float _UJump, _VJump, _Tiling, _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				//顶点与片元相同，同一平面上  (1-t)(b1+Time)+t(b2+Time) = b3 +Time

				//mapping到-1到1
				float2 flowVector = tex2D(_FlowMap,i.uv).rg * 2 - 1;

				float time = _Time.y * _Speed + tex2D(_FlowMap,i.uv).r;

				float2 jump = float2(_UJump, _VJump);

				float3 uvwA = flowUV(i.uv, flowVector, jump, _Tiling,time,false);
				float3 uvwB = flowUV(i.uv, flowVector, jump, _Tiling,time, true);

				fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
				fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;

           
                return fixed4(texA + texB);
            }
            ENDCG
        }
    }
}
