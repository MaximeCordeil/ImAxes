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
                #pragma multi_compile_instancing
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
					float3 normal:	NORMAL;  // [x: index ||| y: size ||| z: not used]
					float4 tangent : TANGENT; // [x: 0=notFiltered, 1=isFiltered ||| y : not used ||| z: 0=notHighlighted, 1=isHighlighted]

                    UNITY_VERTEX_INPUT_INSTANCE_ID
        		};

				struct GS_INPUT
				{
					float4	pos		: POSITION;
					float3	normal	: NORMAL;
					float2  tex0	: TEXCOORD0;
					float4  color		: COLOR;
					float4 flags	: TEXCOORD1; // [x: 0=notFiltered, 1=isFiltered ||| y : not used ||| z: 0=notHighlighted, 1=isHighlighted, w: isBrushed]

					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				struct FS_INPUT
				{
					float4	pos		: POSITION;
					float2  tex0	: TEXCOORD0;
					float4  color		: COLOR;
					float3	normal	: NORMAL;
					float4 flags	: TEXCOORD1;  // [x: 0=notFiltered, 1=isFiltered ||| y : not used ||| z: 0=notHighlighted, 1=isHighlighted, w: isBrushed]

                    UNITY_VERTEX_OUTPUT_STEREO
				};

				struct F_OUTPUT
				{
                    float4 color : COLOR;
                    float depth : SV_Depth;
				};

				// **************************************************************
				// Variables													*
				// **************************************************************

                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(float, _Size)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MinSize)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MaxSize)

                    UNITY_DEFINE_INSTANCED_PROP(float, _BrushSize)

                    UNITY_DEFINE_INSTANCED_PROP(float, _MinX)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MaxX)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MinY)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MaxY)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MinZ)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MaxZ)

                    UNITY_DEFINE_INSTANCED_PROP(float, _MinNormX)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormX)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MinNormY)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormY)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MinNormZ)
                    UNITY_DEFINE_INSTANCED_PROP(float, _MaxNormZ)
                UNITY_INSTANCING_BUFFER_END(Props)

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
					GS_INPUT output;

					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_OUTPUT(GS_INPUT, output);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
					UNITY_TRANSFER_INSTANCE_ID(v, output);

					// Access instanced variables
					float MinNormX = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormX);
					float MaxNormX = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormX);
					float MinNormY = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormY);
					float MaxNormY = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormY);
					float MinNormZ = UNITY_ACCESS_INSTANCED_PROP(Props, _MinNormZ);
					float MaxNormZ = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxNormZ);
					float MinX = UNITY_ACCESS_INSTANCED_PROP(Props, _MinX);
					float MaxX = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxX);
					float MinY = UNITY_ACCESS_INSTANCED_PROP(Props, _MinY);
					float MaxY = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxY);
					float MinZ = UNITY_ACCESS_INSTANCED_PROP(Props, _MinZ);
					float MaxZ = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxZ);

					//lookup the texture to see if the vertex is brushed...
					float2 indexUV = float2((v.normal.x % _DataWidth) / _DataWidth, 1.0 - ((v.normal.x / _DataWidth) / _DataHeight));
					float4 brushValue = tex2Dlod(_MainTex, float4(indexUV, 0.0, 0.0));

					//TODO LATER: THIS REMAPS THE RANGE OF VALUES
					float3 normalisedPosition = float3(
						normaliseValue(v.position.x, MinNormX, MaxNormX ,-0.45, 0.45),
						normaliseValue(v.position.y, MinNormY, MaxNormY ,-0.45, 0.45),
						normaliseValue(v.position.z, MinNormZ, MaxNormZ ,-0.45, 0.45));

//					output.pos = ObjectToWorldDistort3d(v.position);// normalisedPosition);
					output.pos = ObjectToWorldDistort3d(normalisedPosition);

					//the normal buffer carries the index of each vertex
					output.tex0 = float2(0, 0);

					output.color = v.color;

					//filtering
					if (v.tangent.x > 0)
					{
						output.flags.x = 1;
					}
					else if (v.position.x <= MinX ||
							v.position.x >= MaxX ||
							v.position.y <= MinY ||
							v.position.y >= MaxY ||
							v.position.z <= MinZ ||
							v.position.z >= MaxZ ||

							normalisedPosition.x < -0.5 ||
							normalisedPosition.x > 0.5 ||
							normalisedPosition.y < -0.5 ||
							normalisedPosition.y > 0.5 ||
							normalisedPosition.z < -0.5 ||
							normalisedPosition.z > 0.5)
					{
						output.flags.x = 1;
					}
					else
					{
						output.flags.x = 0;
					}

					output.normal = v.normal;
					// isBrushed
					output.flags.w = brushValue.r;
					// isHighlighted
					output.flags.z = v.tangent.z;

					return output;
				}



				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(4)]
				void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
				{
					if (!(p[0].flags.x > 0))
					{
						FS_INPUT pIn;

						UNITY_SETUP_INSTANCE_ID(p[0]);
						UNITY_INITIALIZE_OUTPUT(FS_INPUT, pIn);
						UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(p[0]);

						pIn.flags.z = p[0].flags.z;
						pIn.flags.w = p[0].flags.w;

						// Access instanced variables
						float Size = UNITY_ACCESS_INSTANCED_PROP(Props, _Size);
						float MinSize = UNITY_ACCESS_INSTANCED_PROP(Props, _MinSize);
						float MaxSize = UNITY_ACCESS_INSTANCED_PROP(Props, _MaxSize);

						float3 up = float3(0, 1, 0);
						float brushSizeFactor = 1.0;
						if(p[0].flags.w > 0) brushSizeFactor = 1.5;

						float3 look = _WorldSpaceCameraPos - p[0].pos;
						//look.y = 0;
						look = normalize(look);

						float3 right = cross(up, look);
						float sizeFactor = normaliseValue(p[0].normal.y, 0.0, 1.0, MinSize, MaxSize);
						float halfS = 0.01f * Size*sizeFactor * p[0].normal.y;

						float4 v[4];

						v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
						v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
						v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
						v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);

						float4x4 vp = UNITY_MATRIX_VP;

						pIn.color = p[0].color;
						pIn.normal = p[0].normal;

						pIn.pos = mul(vp, v[0]);
						pIn.tex0 = float2(1.0f, 0.0f);
						pIn.normal = p[0].normal;
						UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
						triStream.Append(pIn);

						pIn.pos = mul(vp, v[1]);
						pIn.tex0 = float2(1.0f, 1.0f);
						pIn.normal = p[0].normal;
						UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
						triStream.Append(pIn);

						pIn.pos =  mul(vp, v[2]);
						pIn.tex0 = float2(0.0f, 0.0f);
						pIn.normal = p[0].normal;
						UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
						triStream.Append(pIn);

						pIn.pos =  mul(vp, v[3]);
						pIn.tex0 = float2(0.0f, 1.0f);
						pIn.normal = p[0].normal;
						UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(p[0], pIn);
						triStream.Append(pIn);

						triStream.RestartStrip();
					}
				}

				// Fragment Shader -----------------------------------------------
				F_OUTPUT FS_Main(FS_INPUT input)
				{
					F_OUTPUT output;

					UNITY_INITIALIZE_OUTPUT(F_OUTPUT, output);
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

					float dx = input.tex0.x - 0.5f;
					float dy = input.tex0.y - 0.5f;
					float dt = dx * dx + dy * dy;
					output.depth = input.pos.z;

					if( dt <= 0.2f)
					{
						if (input.flags.z > 0)
							output.color = float4(0.5, 0, 0.5, 1);
						else
							output.color = float4(input.color.x-dt*0.15,input.color.y-dt*0.15,input.color.z-dt*0.15,0.75);

						if (input.flags.z < 0)
							output.color.a = 0.1;
						return output;
					}
					else if(dx * dx + dy * dy <= 0.21f)
					{
						output.color = float4(0.0, 0.0, 0.0, 1.0);
						return output;
					}
					else
					{
						discard;
						output.color = float4(0.1, 0.1, 0.1, 1.0);
						return output;
					}
				}

			ENDCG

	}


}
}
