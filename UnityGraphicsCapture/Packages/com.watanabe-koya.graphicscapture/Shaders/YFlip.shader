Shader "GraphicsCapture/YFlip"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
     }

    SubShader
    {
        Pass
        {
            Name "YFlip"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _UV;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;
                OUT.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
#if UNITY_UV_STARTS_AT_TOP
                OUT.uv.y = 1.0 - OUT.uv.y;
#endif
                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                return tex2D(_MainTex, IN.uv);
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}
