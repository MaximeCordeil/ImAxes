// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexColor" {
    SubShader {
    Pass {
        LOD 200
         
        CGPROGRAM
        
        #pragma vertex vert
        #pragma fragment frag
 		
		#include "UnityCG.cginc"
 		
        struct VertexInput {
            float4 v : POSITION;
            float4 color: COLOR;
        };
         
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float size : PSIZE;
            float4 col : COLOR;
        };
         
        VertexOutput vert(VertexInput v) {
         
            VertexOutput o;
            o.pos = UnityObjectToClipPos(v.v);
            float4 colOut = v.color;
            colOut.w = 0.1;
            
            o.col = float4(1.0,0.0,0.0,0.2);
            o.size = 3;
             
            return o;
        }
        
        float4 frag(VertexOutput o) : COLOR {
				
				VertexOutput toReturn = o;
				
				float2 centre = float2(o.pos.x/2., o.pos.y/2.) ;
				float radius = 1.0;
				
				float2 position = o.pos.xy - centre;

			if (length(position) > radius)
			{
  				toReturn.col = float4(0.0,0.0,0.0, 1.0);
			}
			else
			{
  				toReturn.col = float4(1.0,1.0,1.0, 1.0);
			}
				//float2 
                return toReturn.col;
        }
 
        ENDCG
        } 
    }
 
}
