Shader "Custom/GradientWavesShaderUI"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color1 ("Gradient Color 1", Color) = (1, 0, 0, 1)
        _Color2 ("Gradient Color 2", Color) = (1, 1, 0, 1)
        _Color3 ("Gradient Color 3", Color) = (0, 1, 0, 1)
        _Color4 ("Gradient Color 4", Color) = (0, 0, 1, 1)
        _Color5 ("Gradient Color 5", Color) = (1, 0, 1, 1)
        _WaveFrequency ("Wave Frequency", Float) = 1.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.5
        _WaveSpeed ("Wave Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _WaveSpeed;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate time-based values
                float time = _Time.y * _WaveSpeed;

                // Apply sine wave to UV coordinates
                float wave = sin(i.uv.x * _WaveFrequency + time) * _WaveAmplitude;

                // Modify UV coordinates with wave effect
                float2 uv = i.uv + float2(0.0, wave);

                // Sample texture color
                fixed4 texColor = tex2D(_MainTex, uv);

                // Calculate gradient factor
                float gradientFactor = frac(i.uv.y + wave);

                // Determine segment of gradient
                float segment = gradientFactor * 5.0; // 5 segments for 5 colors
                float blend = fmod(segment, 1.0); // Fractional part for interpolation

                // Interpolate between the colors
                fixed4 gradientColor;
                if (segment < 1.0)
                    gradientColor = lerp(_Color1, _Color2, blend);
                else if (segment < 2.0)
                    gradientColor = lerp(_Color2, _Color3, blend);
                else if (segment < 3.0)
                    gradientColor = lerp(_Color3, _Color4, blend);
                else if (segment < 4.0)
                    gradientColor = lerp(_Color4, _Color5, blend);
                else
                    gradientColor = lerp(_Color5, _Color1, blend); // Loop back to start

                // Combine texture and gradient
                fixed4 finalColor = texColor * gradientColor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
