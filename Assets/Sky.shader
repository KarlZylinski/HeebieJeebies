Shader "Unlit/Sky"
{
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
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
				float l = sin(1-length(i.vertex_org.xy*0.71f)/(1/23.0f));
				float e = l;
				float2 p = i.uv - _WorldSpaceCameraPos.xy * 0.001;
				float ns = snoise(p * 10.0f) * 0.5f;
				float c = e + e * ns;
				return fixed4(c * 0.6, c * 0.6, c * 0.98, e);
			}
			ENDCG
		}
	}
}
