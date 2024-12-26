Shader "Custom/UI/ScrollingTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling", Vector) = (1, 1, 0, 0)
        _ScrollSpeed ("Scroll Speed", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Tags { "LightMode"="Always" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tiling;
            float4 _ScrollSpeed;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _Tiling.xy;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float time = _Time.y; // unity_Time.y é o tempo em segundos
                float2 scrolledUV = i.uv + _ScrollSpeed.xy * time;
                scrolledUV = frac(scrolledUV);
                return tex2D(_MainTex, scrolledUV);
            }
            ENDCG
        }
    }
    FallBack "Transparent"
}
