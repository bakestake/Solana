Shader "Custom/DrunkEffect_URP"
{
    Properties
    {
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.1
        _WaveFrequency ("Wave Frequency", Range(1, 10)) = 3.0
        _AberrationStrength ("Aberration Strength", Range(0, 1)) = 0.1
        _RotationSpeed ("Aberration Rotation Speed", Range(0, 10)) = 1.0
        _EffectWeight ("Effect Weight", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Name "DrunkEffect"
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float _DistortionStrength;
            float _WaveFrequency;
            float _AberrationStrength;
            float _RotationSpeed;
            float _EffectWeight;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float2 GetRotatingAberrationOffset(float2 uv, float strength, float angleOffset)
            {
                float angle = _Time.y * _RotationSpeed + angleOffset;
                float2 offset = float2(cos(angle), sin(angle)) * strength;
                return uv + offset;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float2 originalUV = uv;

                // Wave distortion
                uv.x += sin(uv.y * _WaveFrequency + _Time.y * 5) * _DistortionStrength;
                uv.y += cos(uv.x * _WaveFrequency + _Time.y * 4) * _DistortionStrength;

                // Aberration offsets
                float2 uvR = GetRotatingAberrationOffset(uv, _AberrationStrength, 0.0);
                float2 uvG = GetRotatingAberrationOffset(uv, _AberrationStrength, 2.0);
                float2 uvB = GetRotatingAberrationOffset(uv, _AberrationStrength, 4.0);

                // Sample with aberration
                half4 color;
                color.r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uvR).r;
                color.g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uvG).g;
                color.b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uvB).b;
                color.a = 1.0;

                // Blend with original
                half4 originalColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, originalUV);
                return lerp(originalColor, color, _EffectWeight);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
