Shader "Custom/DrunkEffect_Noise"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveXStrength ("Noise X Strength", Range(0, 1)) = 0.05
        _WaveYStrength ("Noise Y Strength", Range(0, 1)) = 0.05
        _WaveFrequency ("Noise Frequency", Range(0, 20)) = 5.0
        _WaveSpeed ("Noise Speed", Range(0, 10)) = 2.0
        _ChromaticAberration ("Chromatic Aberration", Range(0, 1)) = 0.1
        _AberrationSpeed ("Aberration Speed", Range(0, 10)) = 0.5
        _EffectWeight ("Effect Weight", Range(0, 1)) = 0.5
        _DoubleVisionStrength ("Double Vision Strength", Range(0, 0.05)) = 0.01
        _DoubleVisionAngle ("Double Vision Angle (Degrees)", Range(0, 360)) = 45.0
    }
    SubShader
    {
        Pass
        {
            ZWrite Off
            ZTest Always

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
            float _WaveXStrength;
            float _WaveYStrength;
            float _WaveFrequency;
            float _WaveSpeed;
            float _ChromaticAberration;
            float _AberrationSpeed;
            float _EffectWeight;
            float _DoubleVisionStrength;
            float _DoubleVisionAngle;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Simple hash function for noise
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            // 2D value noise
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // Noise-based offset
            float2 GetNoiseOffset(float2 uv, float time)
            {
                float n = noise(uv * _WaveFrequency + time * _WaveSpeed);
                float2 offset = float2(
                    (n - 0.5) * _WaveXStrength,
                    (n - 0.5) * _WaveYStrength
                );
                return uv + offset;
            }

            // Chromatic aberration
            float2 GetChromaticAberrationOffset(float2 uv, float strength, float speed)
            {
                float angle = _Time.y * speed;
                float2 offset = float2(cos(angle), sin(angle)) * strength;
                return uv + offset;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = GetNoiseOffset(i.uv, _Time.y);

                // Chromatic aberration
                float2 uvR = GetChromaticAberrationOffset(uv, _ChromaticAberration, _AberrationSpeed);
                float2 uvG = GetChromaticAberrationOffset(uv, _ChromaticAberration, _AberrationSpeed + 0.2);
                float2 uvB = GetChromaticAberrationOffset(uv, _ChromaticAberration, _AberrationSpeed - 0.2);

                float4 color;
                color.r = tex2D(_MainTex, uvR).r;
                color.g = tex2D(_MainTex, uvG).g;
                color.b = tex2D(_MainTex, uvB).b;
                color.a = 1.0;

                // Double vision (ghost image)
                float angleRad = radians(_DoubleVisionAngle);
                float2 ghostOffset = float2(cos(angleRad), sin(angleRad)) * _DoubleVisionStrength;
                float4 ghostColor = tex2D(_MainTex, uv + ghostOffset);
                color = lerp(color, ghostColor, 0.5);

                // Blend with original
                float4 originalColor = tex2D(_MainTex, i.uv);
                return lerp(originalColor, color, _EffectWeight);
            }
            ENDCG
        }
    }
}
