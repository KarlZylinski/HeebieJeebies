Shader "Unlit/StarBackground"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 vertex_org : TEXCOORD1;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex_org = v.vertex;
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{ 
				float2 p = i.uv - _WorldSpaceCameraPos.xy * 0.001;
				float ns = smoothstep(0.9, 0.95, snoise(p * 175.0f)) * 2.0f;
				float var = snoise(p * 10) * 0.25 + 0.4;
				float r = snoise(p.yx * 50) * 0.5 + 0.9;
				float g = snoise(p.xy * 50) * 0.2 + 0.9;
				float b = snoise(p.yx * 90) * 0.3 + 0.9;
				float f = ns * var;
				float gg = smoothstep(0.0f, 0.5, snoise(p.yx * 2) * 0.09 + 0.05);
				float bb = smoothstep(0.0f, 0.8, snoise(p.xy * 2.5f) * 0.09 + 0.03);
				float rr = smoothstep(0.0f, 0.4, snoise(p.xy * 1.5f) * 0.09 + 0.02);
				fixed4 col = fixed4(f * r + rr, f * g + gg, f * b + bb, 1);
				return col;
			}
			ENDCG
		}
	}
}
