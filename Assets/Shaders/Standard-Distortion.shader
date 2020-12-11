Shader "Staxestk/Standard"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		
		_ftl("Front Top Left", Vector) = (-1,1,0,0)
		_ftr("Front Top Right", Vector) = (1,1,0,0)
		_fbl("Front Bottom Left", Vector) = (-1,-1,0,0)
		_fbr("Front Bottom Right", Vector) = (1,-1,0,0)
		_MinX("_MinA",Float) = 0
		_MaxX("_MaxA",Float) = 0
		_MinY("_MinB",Float) = 0
		_MaxY("_MaxB",Float) = 0
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

		#include "UnityCG.cginc"
		#include "ImAxes/Distort.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float4 color: COLOR;
		float3 normal	: NORMAL;
	};

	struct gs_in
	{
		float4 vertex	:	POSITION;
		float4 color	:	COLOR;
		float2 uv		:	TEXCOORD0;
		bool filtered	:	BOOL;
		bool isBrushed	:	BOOL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float4 color: COLOR;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

					
	//*******************
	// RANGE FILTERING
	//*******************

	float _MinA;
	float _MaxA;
	float _MinB;
	float _MaxB;

	// VERTEX SHADER
	gs_in vert(appdata v)
	{
		float4 pos = v.vertex;
		gs_in o;
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		o.color = v.color;// float4(1.0,0.0,0.0,1.0);

		o.isBrushed = false;

		if (v.normal.x == 1.0)
		{
			o.color = float4(1.0, 0.0, 0.0, 1.0);
			o.isBrushed = true;
		}

	    o.filtered = false;

		if(v.vertex.x < 0) // filter with MinA, MaxA
		{
		if(v.vertex.y < _MinA || v.vertex.y > _MaxA)
		{
		//	o.filtered = true;  
		}
		}

		else // filter with MinB, MaxB	
		{
		if(v.vertex.y < _MinB || v.vertex.y > _MaxB)
		{
			//o.filtered = true;  
		}
		}
		o.vertex = mul(unity_ObjectToWorld, float3(pos.x,pos.y,pos.z));// ObjectToProjectionDistort(pos);
		
		return o;
	}

	//GEOMETRY SHADER
	[maxvertexcount(2)]
	void geom (line gs_in l[2], inout LineStream<v2f> lineStream)
	{

		//bool filtered = false;
		bool filtered = (l[0].filtered || l[1].filtered);

		v2f In;
		
		In.color = l[0].color;
		if (l[0].isBrushed || l[1].isBrushed)
			In.color = float4(1.0, 0.0, 0.0, 1.0);
		if (filtered)
			In.color = float4(0.1, 0.1, 0.1, 1.0);

		In.vertex = l[0].vertex;
		In.uv = l[0].uv;
		lineStream.Append(In);
		

		In.color = l[1].color;
		if (l[0].isBrushed || l[1].isBrushed)
			In.color = float4(1.0, 0.0, 0.0, 1.0);
		if (filtered)
			In.color = float4(0.1, 0.1, 0.1, 1.0);

		In.vertex = l[1].vertex;
		In.uv = l[1].uv;
		lineStream.Append(In);

	}

	//FRAGMENT SHADER
	fixed4 frag(v2f i) : SV_Target
	{
		return i.color;
	}
		ENDCG
	}
	}
}