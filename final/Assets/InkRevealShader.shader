Shader "Unlit/NewUnlitShader"
{
   Properties
    {
        _BaseTex("Base Texture", 2D) = "white" {}
        _InkTex("Ink Texture", 2D) = "black" {}
        _RevealMask("Reveal Mask", 2D) = "white" {}
        _RevealPos("Reveal Position", Vector) = (0, 0, 0, 0)
        _RevealRadius("Reveal Radius", Float) = 0.1
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
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _BaseTex;
            sampler2D _InkTex;
            sampler2D _RevealMask;
            float4 _BaseTex_ST;
            float4 _InkTex_ST;

            float4 _RevealPos;
            float _RevealRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.uv, _RevealPos.xy);
                float mask = smoothstep(_RevealRadius, _RevealRadius * 0.5, dist);

                fixed4 baseCol = tex2D(_BaseTex, i.uv);
                fixed4 inkCol = tex2D(_InkTex, i.uv);

                fixed4 finalCol = lerp(inkCol, baseCol, mask);

                UNITY_APPLY_FOG(i.fogCoord, finalCol);
                return finalCol;
            }
            ENDCG
        }
    }
}