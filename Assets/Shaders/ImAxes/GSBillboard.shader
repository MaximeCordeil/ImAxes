// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Outline Dots" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "White" {}
		_Size ("Size", Range(0, 30)) = 0.5
		_BrushSize("BrushSize",Float) = 0.05
		_MinX("_MinX",Float) = 0
		_MaxX("_MaxX",Float) = 0
		_MinY("_MinY",Float) = 0
		_MaxY("_MaxY",Float) = 0
		_data_size("data_size",Float) = 0
		_tl("Top Left", Vector) = (-1,1,0,0)
		_tr("Top Right", Vector) = (1,1,0,0)
		_bl("Bottom Left", Vector) = (-1,-1,0,0)
		_br("Bottom Right", Vector) = (1,-1,0,0)
	}

	SubShader 
	{
		Pass
		{
			Name "Onscreen geometry"
			Tags { "RenderType"="Transparent" }
			//Blend func : Blend Off : turns alpha blending off
			Blend SrcAlpha OneMinusSrcAlpha
			//Lighting On
			Zwrite On
			//ZTest NotEqual    
            //Cull Front
			LOD 200
		
			CGPROGRAM
				//// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
				//#pragma exclude_renderers d3d11 gles
				#pragma target 5.0
				#pragma vertex VS_Main
				#pragma fragment FS_Main
				#pragma geometry GS_Main
				#include "UnityCG.cginc" 
				#include "Distort.cginc"
				#include "Helper.cginc"

				// **************************************************************
				// Data structures												*
				// **************************************************************
				
				//float brusedIndices[65000];

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
					float4  color		: COLOR;
					float	isBrushed : FLOAT;
				};

				struct FS_INPUT
				{
					float4	pos		: POSITION;
					float2  tex0	: TEXCOORD0;
					//float2	tex1	: TEXCOORD1;
					float4  color		: COLOR;
					float	isBrushed : FLOAT;
					float3	normal	: NORMAL;
				};


				// **************************************************************
				// Vars															*
				// **************************************************************

				float _Size;
				float _MinSize;
				float _MaxSize;
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

				// ********************
				// Normalisation ranges
				// ********************

				float _MinNormX;
				float _MaxNormX;
				float _MinNormY;
				float _MaxNormY;
				float _MinNormZ;
				float _MaxNormZ;

				float4x4 _VP;
				sampler2D _MainTex;
				//Sampler2D _MainTexSampler;

				//SamplerState sampler_MainTex;
				
				float _DataWidth;
				float _DataHeight;

				//float[] brushedIndexes;
				
				// **************************************************************
				// Shader Programs												*
				// **************************************************************

				// Vertex Shader ------------------------------------------------
				GS_INPUT VS_Main(VS_INPUT v)
				{
					GS_INPUT output = (GS_INPUT)0;
					
					//lookup the texture to see if the vertex is brushed...
					float2 indexUV = float2((v.normal.x % _DataWidth) / _DataWidth, 1.0 - ((v.normal.x / _DataWidth) / _DataHeight));
					float4 brushValue = tex2Dlod(_MainTex, float4(indexUV, 0.0, 0.0));

					output.isBrushed = brushValue.r;

					//TODO LATER: THIS REMAPS THE RANGE OF VALUES
					float3 normalisedPosition = float3(
						normaliseValue(v.position.x, _MinNormX, _MaxNormX ,-0.45, 0.45),
						normaliseValue(v.position.y, _MinNormY, _MaxNormY ,-0.45, 0.45),
						normaliseValue(v.position.z, _MinNormZ, _MaxNormZ ,-0.45, 0.45));

//					output.pos = ObjectToWorldDistort3d(v.position);// normalisedPosition);
					output.pos = ObjectToWorldDistort3d(normalisedPosition);

					//the normal buffer carries the index of each vertex
					output.tex0 = float2(0, 0);

					output.color = v.color;
					output.isBrushed= 0.0;
			
					//filtering
					if (v.position.x <= _MinX ||
					 v.position.x >= _MaxX || 
					 v.position.y <= _MinY || 
					 v.position.y >= _MaxY || 
					 v.position.z <= _MinZ || 
					 v.position.z >= _MaxZ 	||

					 normalisedPosition.x < -0.5 ||
					 normalisedPosition.x > 0.5 || 
					 normalisedPosition.y < -0.5 || 
					 normalisedPosition.y > 0.5 || 
					 normalisedPosition.z < -0.5 || 
					 normalisedPosition.z > 0.5			 
					 )
					{
						output.color.w = 0;
					}
					output.normal = v.normal;
					return output;
				}



				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(4)]
				void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
				{
					float3 up = float3(0, 1, 0);
					float brushSizeFactor = 1.0;
					//if (p[0].isBrushed == 1.0) brushSizeFactor = 1.2;
					if(p[0].isBrushed > 0.0) brushSizeFactor = 1.5;

					float3 look = _WorldSpaceCameraPos - p[0].pos;
					//look.y = 0;
					look = normalize(look);

					float3 right = cross(up, look);
					float sizeFactor = normaliseValue(p[0].normal.y, 0.0, 1.0, _MinSize, _MaxSize);
					float halfS = 0.01f * _Size*sizeFactor * p[0].normal.y;
							
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
				float4 FS_Main(FS_INPUT input) : SV_Target0
				{
					//FragmentOutput fo = (FragmentOutput)0;

					float dx = input.tex0.x - 0.5f;
					float dy = input.tex0.y - 0.5f;

					float dt = dx * dx + dy * dy;
					
					if(input.color.w == 0)
					{
						//if( dt <= 0.2f)
						//	return float4(0.1,0.1,0.1,1.0);
						//else
						//	if(dx * dx + dy * dy <= 0.25f)
						//	return float4(0.0, 0.0, 0.0, 1.0);
						//	else
						//	{
							discard;
							return float4(0.0, 0.0, 0.0, 0.0);
//							}
					}
					else
					{
					if( dt <= 0.2f)
					{
						//if(input.isBrushed==1.0)
						//return float4(1.0,0.0,0.0,1.0);
						//else
						return float4(input.color.x-dt*0.15,input.color.y-dt*0.15,input.color.z-dt*0.15,0.8);
					}// float4(input.color.x-dt*0.25,input.color.y-dt*0.25,input.color.z-dt*0.25,1.0);
					else
					if(dx * dx + dy * dy <= 0.21f)
					return float4(0.0, 0.0, 0.0, 1.0);
					else
					{
					discard;	
					return float4(0.1, 0.1, 0.1, 1.0);
					}
					}

					//return fo;
				}

			ENDCG
	
	}

	
}
}
