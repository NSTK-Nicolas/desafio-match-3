Shader "Custom/CircularGradientShaderUI"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color1 ("Gradient Color 1", Color) = (1, 0, 0, 1)
        _Color2 ("Gradient Color 2", Color) = (0, 1, 0, 1)
        _Color3 ("Gradient Color 3", Color) = (0, 0, 1, 1)
        _Color4 ("Gradient Color 4", Color) = (1, 1, 0, 1)
        _Color5 ("Gradient Color 5", Color) = (1, 0, 1, 1)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _CircleFrequency ("Circle Frequency", Float) = 5.0
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
            float _WaveSpeed;
            float _CircleFrequency;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Center the UV coordinates around (0.5, 0.5)
                float2 centeredUV = i.uv - 0.5;

                // Calculate the radial distance from the center
                float radialDistance = length(centeredUV);

                // Add a time-based wave effect to the radial distance
                float time = _Time.y * _WaveSpeed;
                radialDistance += sin(radialDistance * _CircleFrequency + time) * 0.05;

                // Normalize the radial distance to create a repeating pattern
                float gradientFactor = frac(radialDistance);

                // Determine the segment of the gradient
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

                // Sample texture color
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Combine the gradient color with the texture
                fixed4 finalColor = texColor * gradientColor;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
