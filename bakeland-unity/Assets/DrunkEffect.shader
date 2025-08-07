Shader "Custom/DrunkEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.1
        _WaveFrequency ("Wave Frequency", Range(1, 10)) = 3.0
        _AberrationStrength ("Aberration Strength", Range(0, 1)) = 0.1
        _RotationSpeed ("Aberration Rotation Speed", Range(0, 10)) = 1.0
        _EffectWeight ("Effect Weight", Range(0, 1)) = 0.0  // New parameter for blending
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
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
            float _DistortionStrength;
            float _WaveFrequency;
            float _AberrationStrength;
            float _RotationSpeed;
            float _EffectWeight;  // New parameter for blending

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Chromatic aberration offset that rotates over time
            float2 GetRotatingAberrationOffset(float2 uv, float strength, float angleOffset)
            {
                float angle = _Time.y * _RotationSpeed + angleOffset;
                float2 offset = float2(cos(angle), sin(angle)) * strength;
                return uv + offset;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Basic UV wave distortion
                float2 uv = i.uv;
                float2 originalUV = uv;  // Save original UV for blending

                uv.x += sin(uv.y * _WaveFrequency + _Time.y * 5) * _DistortionStrength;
                uv.y += cos(uv.x * _WaveFrequency + _Time.y * 4) * _DistortionStrength;

                // Apply rotating chromatic aberration (separate RGB channels, rotating over time)
                float2 uvR = GetRotatingAberrationOffset(uv, _AberrationStrength, 0.0);  
                float2 uvG = GetRotatingAberrationOffset(uv, _AberrationStrength, 2.0);  
                float2 uvB = GetRotatingAberrationOffset(uv, _AberrationStrength, 4.0);  

                // Sample the texture with the aberration effect
                float4 color;
                color.r = tex2D(_MainTex, uvR).r;
                color.g = tex2D(_MainTex, uvG).g;
                color.b = tex2D(_MainTex, uvB).b;
                color.a = 1.0;

                // Blend the distorted color with the original using the effect weight
                float4 originalColor = tex2D(_MainTex, originalUV);
                color = lerp(originalColor, color, _EffectWeight);

                return color;
            }
            ENDCG
        }
    }
}
