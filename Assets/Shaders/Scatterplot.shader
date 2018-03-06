// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
Shader "Custom/Scatterplot" 
{
    Properties 
	{
        _MainTex ("Base (RGB)", 2D) = "White" {
            }
		_Size ("Size", Range(0, 30)) = 0.5
		_IndexPos ("IndexPos", Vector) = (15.0,15.0,15.0,15.0)
		_X("X",Float) = 15.0
		_Y("Y",Float) = 15.0
		_Z("Z",Float) = 15.0
		_BrushSize("BrushSize",Float) = 0.05
		_MinX("_MinX",Float) = 0
		_MaxX("_MaxX",Float) = 0
		_MinY("_MinY",Float) = 0
		_MaxY("_MaxY",Float) = 0

		_tl("Top Left", Vector) = (-1,1,0,0)
		_tr("Top Right", Vector) = (1,1,0,0)
		_bl("Bottom Left", Vector) = (-1,-1,0,0)
		_br("Bottom Right", Vector) = (1,-1,0,0)

	}

	SubShader 
	{
        Pass
		{
            Tags {
                "RenderType"="Transparent" }
			//Blend func : Blend Off : turns alpha blending off
			Blend SrcAlpha OneMinusSrcAlpha
			//Lighting On
			Zwrite On
			//ZTest NotEqual    
            //Cull Front
			LOD 200
		
			CGPROGRAM
				// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
				#pragma exclude_renderers d3d11 gles
				#pragma target 5.0
				#pragma vertex VS_Main
				#pragma fragment FS_Main
				#pragma geometry GS_Main
				#include "UnityCG.cginc" 
				#include "Distort.cginc"

				// **************************************************************
				// Data structures												*
				// **************************************************************
				
		        struct VS_INPUT {
                float4 position : POSITION;
                float4 color: COLOR;
                float3 normal:	NORMAL;
            };
            struct GS_INPUT
				{
                float4	pos		: POSITION;
                float3	normal	: NORMAL;
                float2  tex0	: TEXCOORD0;
                float4  color		: FLOAT;
                float	isBrushed : BOOL;
            };
            struct FS_INPUT
				{
                float4	pos		: POSITION;
                float2  tex0	: TEXCOORD0;
                float4  color		: COLOR;
                float	isBrushed : FLOAT;
                float3	normal	: NORMAL;
            };
            // **************************************************************
				// Vars															*
				// **************************************************************

				float _Size;
            float4 _IndexPos;
            float _X;
            float _Y;
            float _Z;
            float _BrushSize;
            //*******************
				// RANGE FILTERING
				//*******************

				float _MinX;
            float _MaxX;
            float _MinY;
            float _MaxY;
            float _MinZ;
            float _MaxZ;
            float4x4 _VP;
            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float[] brushedIndexes;
            //*********************************
				// helper functions
				//*********************************

				float normaliseValue(float value, float i0, float i1, float j0, float j1)
				{
                float L = (j0 - j1) / (i0 - i1);
                return (j0 - (L * i0) + (L * value));
            }


		

				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				GS_INPUT VS_Main(VS_INPUT v)
				{
                GS_INPUT output = (GS_INPUT)0;
                //output.pos = mul(unity_ObjectToWorld, v.position);
					output.pos = ObjectToWorldDistort3d(v.position);
                //the normal buffer carries the index of each vertex
					output.tex0 = float2(0, 0);
					output.isBrushed = 0.0;
                output.color = v.color;
                if(v.position.x <= _MinX || v.position.x >= _MaxX || v.position.y <= _MinY || v.position.y >= _MaxY)// || v.position.z <= _MinZ || v.position.z >= _MaxZ)
					output.color.w = 0;
                output.normal = v.normal;
                return output;
            }




				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(4)]
				void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
				{
                float3 up = float3(0, 1, 0);
                float3 look = _WorldSpaceCameraPos - p[0].pos;
                //look.y = 0;
					look = normalize(look);
                float3 right = cross(up, look);
                float halfS = 0.01f * _Size;
                float4 v[4];
                v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
                v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
                v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
                v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);
                float4x4 vp = UNITY_MATRIX_VP;
                FS_INPUT pIn;
                pIn.isBrushed = p[0].isBrushed;
                pIn.color = p[0].color;
                pIn.normal = p[0].normal;
                pIn.pos = mul(vp, v[0]);
                pIn.tex0 = float2(1.0f, 0.0f);
                pIn.normal = p[0].normal;
                triStream.Append(pIn);
                pIn.pos = mul(vp, v[1]);
                pIn.tex0 = float2(1.0f, 1.0f);
                pIn.normal = p[0].normal;
                triStream.Append(pIn);
                pIn.pos =  mul(vp, v[2]);
                pIn.tex0 = float2(0.0f, 0.0f);
                pIn.normal = p[0].normal;
                triStream.Append(pIn);
                pIn.pos =  mul(vp, v[3]);
                pIn.tex0 = float2(0.0f, 1.0f);
                pIn.normal = p[0].normal;
                triStream.Append(pIn);
                triStream.RestartStrip();
            }


				// Fragment Shader -----------------------------------------------
				float4 FS_Main(FS_INPUT input) : COLOR
				{

                float dx = input.tex0.x - 0.5f;
                float dy = input.tex0.y - 0.5f;
                float dt = dx * dx + dy * dy;
                if(input.color.w == 0)
					{
                    discard;
                    return float4(0.0, 0.0, 0.0, 0.0f);
                }

				else{
                if( dt <= 0.2f)
					return float4(input.color.x-dt*0.25,input.color.y-dt*0.25,input.color.z-dt*0.25,1.0);
                else
				if(dx * dx + dy * dy <= 0.25f)
				return float4(0.0, 0.0, 0.0, 1.0);
                else
				{
                    discard;
                    return float4(0.0, 0.0, 0.0, 0.0f);
                }


				}

				}

			ENDCG
		}
	} 
}