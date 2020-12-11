Shader "Staxestk/Linked-Views-Material"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_ftl1("Front Top Left Axis 1", Vector) = (-1,1,0,0)
		_ftr1("Front Top Right Axis 1", Vector) = (1,1,0,0)
		_fbl1("Front Bottom Left Axis 1", Vector) = (-1,-1,0,0)
		_fbr1("Front Bottom Right Axis 1", Vector) = (1,-1,0,0)
		_btl1("Back Top Left Axis 1", Vector) = (-1,1,-1,0)
		_btr1("Back Top Right Axis 1", Vector) = (1,1,-1,0)
		_bbl1("Back Bottom Left Axis 1", Vector) = (-1,-1,-1,0)
		_bbr1("Back Bottom Right Axis 1", Vector) = (1,-1,-1,0)

        _ftl2("Front Top Left Axis 2", Vector) = (-1,1,0,0)
        _ftr2("Front Top Right Axis 2", Vector) = (1,1,0,0)
        _fbl2("Front Bottom Left Axis 2", Vector) = (-1,-1,0,0)
        _fbr2("Front Bottom Right Axis 2", Vector) = (1,-1,0,0)
        _btl2("Back Top Left Axis 2", Vector) = (-1,1,-1,0)
        _btr2("Back Top Right Axis 2", Vector) = (1,1,-1,0)
        _bbl2("Back Bottom Left Axis 2", Vector) = (-1,-1,-1,0)
        _bbr2("Back Bottom Right Axis 2", Vector) = (1,-1,-1,0)

	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" }
			//Blend func : Blend Off : turns alpha blending off
			Blend SrcAlpha OneMinusSrcAlpha
			//Lighting On
			Zwrite On
			//ZTest NotEqual
            //Cull Front
		LOD 200

		Pass
	{
			Tags { "RenderType"="Transparent" }

		CGPROGRAM
		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag
				// make fog work
		#pragma multi_compile_fog
		#pragma multi_compile_instancing

		#include "UnityCG.cginc"
		#include "DistortLinked.cginc"
		#include "Helper.cginc"

	struct appdata
	{
		float4 position : POSITION;
		float2 uv : TEXCOORD0;
		float4 color: COLOR;
		float3 normal	: NORMAL;

		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct gs_in
	{
		float4 vertex	:	POSITION;
		float4 color	:	COLOR;
		float2 uv		:	TEXCOORD0;
		bool filtered	:	BOOL;
		bool isBrushed	:	BOOL;

		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct g2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float4 color: COLOR;

		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct f_output
	{
		float4 color : COLOR;
		float depth : SV_Depth;
	};


	// **************************************************************
	// Variables													*
	// **************************************************************

	UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinXFilter1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxXFilter1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinYFilter1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxYFilter1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinZFilter1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxZFilter1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinXFilter2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxXFilter2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinYFilter2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxYFilter2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinZFilter2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxZFilter2)

		UNITY_DEFINE_INSTANCED_PROP(float, _MinNormX1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormX1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinNormY1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormY1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinNormZ1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormZ1)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinNormX2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormX2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinNormY2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormY2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MinNormZ2)
		UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormZ2)
	UNITY_INSTANCING_BUFFER_END(Props)

	sampler2D _MainTex;
	float4 _MainTex_ST;


	// VERTEX SHADER
	gs_in vert(appdata v)
	{
		gs_in o;

		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_OUTPUT(gs_in, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		UNITY_TRANSFER_INSTANCE_ID(v, o);

		// Access instanced variables
		float MinXFilter1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinXFilter1);
		float MaxXFilter1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxXFilter1);
		float MinYFilter1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinYFilter1);
		float MaxYFilter1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxYFilter1);
		float MinZFilter1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinZFilter1);
		float MaxZFilter1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxZFilter1);
		float MinXFilter2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinXFilter2);
		float MaxXFilter2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxXFilter2);
		float MinYFilter2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinYFilter2);
		float MaxYFilter2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxYFilter2);
		float MinZFilter2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinZFilter2);
		float MaxZFilter2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxZFilter2);
		float MinNormX1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormX1);
		float MaxNormX1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormX1);
		float MinNormY1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormY1);
		float MaxNormY1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormY1);
		float MinNormZ1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormZ1);
		float MaxNormZ1 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormZ1);
		float MinNormX2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormX2);
		float MaxNormX2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormX2);
		float MinNormY2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormY2);
		float MaxNormY2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormY2);
		float MinNormZ2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormZ2);
		float MaxNormZ2 = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormZ2);

		float4 pos = v.position;
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.color = v.color;

		float3 normalisedPosition;

		if(v.normal.z == 0.0)
		{
		normalisedPosition = float3(
						normaliseValue(v.position.x, MinNormX1, MaxNormX1 ,-0.45, 0.45),
						normaliseValue(v.position.y, MinNormY1, MaxNormY1 ,-0.45, 0.45),
						normaliseValue(v.position.z, MinNormZ1, MaxNormZ1 ,-0.45, 0.45));

		if (v.position.x <= MinXFilter1 ||
			v.position.x >= MaxXFilter1 ||
			v.position.y <= MinYFilter1 ||
			v.position.y >= MaxYFilter1 ||
			v.position.z <= MinZFilter1 ||
			v.position.z >= MaxZFilter1 ||

			normalisedPosition.x < -0.45 ||
			normalisedPosition.x > 0.45 ||
			normalisedPosition.y < -0.45 ||
			normalisedPosition.y > 0.45 ||
			normalisedPosition.z < -0.45 ||
			normalisedPosition.z > 0.45
			)
			{
			o.filtered = true;
			//o.color.w=0;
			}
			else o.filtered = false;
		}
		else if(v.normal.z == 1.0)
		{
		normalisedPosition = float3(
						normaliseValue(v.position.x, MinNormX2, MaxNormX2 ,-0.45, 0.45),
						normaliseValue(v.position.y, MinNormY2, MaxNormY2 ,-0.45, 0.45),
						normaliseValue(v.position.z, MinNormZ2, MaxNormZ2 ,-0.45, 0.45));

		if (v.position.x <= MinXFilter2 ||
			v.position.x >= MaxXFilter2 ||
			v.position.y <= MinYFilter2 ||
			v.position.y >= MaxYFilter2 ||
			v.position.z <= MinZFilter2 ||
			v.position.z >= MaxZFilter2 ||

			normalisedPosition.x < -0.45 ||
			normalisedPosition.x > 0.45 ||
			normalisedPosition.y < -0.45 ||
			normalisedPosition.y > 0.45 ||
			normalisedPosition.z < -0.45 ||
			normalisedPosition.z > 0.45)
			{
			o.filtered = true;
			//o.color.w=0;
			}else o.filtered = false;
		}

		o.vertex = mul(UNITY_MATRIX_VP, ObjectToWorldDistort3d(normalisedPosition, v.normal.z > 0));
		//o.vertex = UnityObjectToClipPos(pos);
		o.isBrushed=false;
		return o;
	}

	//GEOMETRY SHADER
	[maxvertexcount(2)]
	void geom (line gs_in l[2], inout LineStream<g2f> lineStream)
	{
		//bool filtered = false;
		bool filtered = (l[0].filtered || l[1].filtered);

		if(!filtered)
		{
			g2f In;

			UNITY_INITIALIZE_OUTPUT(g2f, In);
			UNITY_SETUP_INSTANCE_ID(l[0]);
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(l[0]);

			In.color = l[0].color;
			In.vertex = l[0].vertex;
			In.uv = l[0].uv;
			UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(l[0], In);
			lineStream.Append(In);

			In.color = l[1].color;
			In.vertex = l[1].vertex;
			In.uv = l[1].uv;
			UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(l[0], In);

			lineStream.Append(In);
		}
	}

	//FRAGMENT SHADER
	f_output frag(g2f i)
	{
		f_output o;

		UNITY_INITIALIZE_OUTPUT(f_output, o);
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		o.color = i.color;
		o.depth = i.vertex.z;
		return o;
	}
		ENDCG
	}
	}
}