// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Perspective2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			
			#include "UnityCG.cginc"
            
            sampler2D _MainTex;
            
            // Values passed from Vertex to Pixel Shader
            struct v2f {
                float4 pos : SV_POSITION;
                float4 pos_frag : TEXCOORD0;
            };

            // Vertex Shader
            v2f vert(appdata_base v) {
                v2f o;
                float4 clipSpacePosition = UnityObjectToClipPos(v.vertex);
                o.pos = clipSpacePosition;
                // Copy of clip space position for fragment shader    
                o.pos_frag = clipSpacePosition;
                return o;
            }

            // Pixel Shader
            half4 frag(v2f i) : SV_Target {
                // Perspective divide (Translate to NDC - (-1, 1))
                float2 uv = i.pos_frag.xy / i.pos_frag.w;
                // Map -1, 1 range to 0, 1 tex coord range
                uv = (uv + float2(1.0, 1.0)) * 0.5;
                return tex2D(_MainTex, uv);
            }
			ENDCG
		}
	}
}
