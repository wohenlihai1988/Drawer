Shader "Unlit/ScreenMask"
{
	Properties
	{
		_FrontCol("Front Color", color) = (1, 1,1 ,1)
		_MainTex("Texture", 2D) = "white" {}
		_BGTex("Background", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _BGTex;
			float4 _MainTex_ST;
			float4 _MaskTex_ST;
			float4 _BGTex_ST;
			float4 _FrontCol;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 bgcol = tex2D(_BGTex, i.uv);
				fixed4 maskcol = tex2D(_MaskTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				fixed4 col = lerp(_FrontCol + bgcol, bgcol, maskcol.r);
				col.a = 0;
				return col;
			}
			ENDCG
		}
	}
}
