// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Custom/Perspective" 
 {
     Properties 
     {
       _MainTex ("Texture", 2D) = "white" {}
     }
     
     SubShader 
     {
        Tags { "RenderType" = "Opaque" }
       
        Pass {
            CGPROGRAM
            //#pragma surface surf Lambert nolightmap noshadow
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
            
            struct Input {
                float4 screenPos : SV_POSITION;
                float4 pos_frag : TEXCOORD0;
            };

            sampler2D _MainTex;

            //void surf (Input IN, inout SurfaceOutput o) {
            //    o.Albedo = tex2D (_MainTex, IN.screenPos.xy / IN.screenPos.w).rgb;
            //}
            
            // Vertex Shader
            Input vert(appdata_base v) {
                Input o;
                float4 clipSpacePosition = UnityObjectToClipPos(v.vertex);
                //float4 clipSpacePosition = v.vertex;
                o.screenPos = clipSpacePosition;
                // Copy of clip space position for fragment shader    
                o.pos_frag = clipSpacePosition;
                return o;
            }
            
            // Pixel Shader
            half4 frag(Input i) : SV_Target {
                // Perspective divide (Translate to NDC - (-1, 1))
                //float invX = -i.pos_frag.x;
                
                float2 uv = float2(i.pos_frag.x, -i.pos_frag.y) / i.pos_frag.w;
                // Map -1, 1 range to 0, 1 tex coord range
                uv = (uv + float2(1.0, 1.0)) * 0.5;
                return tex2D(_MainTex, uv);
            }
            
            ENDCG
        }
     } 
     Fallback "Diffuse"
}