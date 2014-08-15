// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile/TransCutout" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader {
	Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
	LOD 100

Pass
    		{
    			Cull Off
    			Lighting Off
    			
    			CGPROGRAM
    			#pragma vertex vert
				#pragma fragment frag
			
				#include "UnityCG.cginc"
    				
    			sampler2D _MainTex;
    			float4 _MainTex_ST;
    
    			sampler2D unity_Lightmap;
    			float4 unity_LightmapST;
    			
    			fixed _Cutoff;
    			
    			struct Vertex
    			{
    				float4 vertex : POSITION;
    				float4 uv : TEXCOORD0;
    				float4 uv2 : TEXCOORD1;
    			};
    
    			struct Fragment
    			{
    				float4 vertex : POSITION;
    				float4 uv : TEXCOORD0;
    				float4 uv2 : TEXCOORD1;
    			};
    
    			Fragment vert(Vertex v)
    			{
    				Fragment o;
    
    				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
    				o.uv.xy = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    				o.uv2.xy = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    
    				return o;
    			}
    												
    			fixed4 frag(Fragment IN) : COLOR
    			{
    				fixed4 output = fixed4(0, 0, 0, 1);
    				
    				output = tex2D(_MainTex, IN.uv.xy);
    				output.rgb *=  DecodeLightmap(tex2D(unity_Lightmap, IN.uv2.xy)).rgb;
    				clip(output.a - _Cutoff);
    				return output;
    			}
    			
    			ENDCG
    		}
    	}
    	
    	SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100
	
	Pass {
		Lighting Off
		Alphatest Greater [_Cutoff]
		SetTexture [_MainTex] { combine texture } 
	}
}

Fallback "Transparent/Cutout/Soft Edge Unlit"
}
