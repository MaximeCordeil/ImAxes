Shader "Unlit/LinkedVisualisation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha("Alpha", float) = 0.5
	}
	SubShader
	{
			Tags { "RenderType"="Transparent" }
			//Blend func : Blend Off : turns alpha blending off
			Blend SrcAlpha OneMinusSrcAlpha
			//Lighting On
			Zwrite On
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
				float3 norm: NORMAL;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color : COLOR;

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float Alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				if(v.norm.x == 1.0) o.color = float4(1.0,0.0,0.0,1.0);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return float4(38.0/255.0, 0.0/255.0, 1.0, 1.0);			            
			}
			ENDCG
		}
	}
}
