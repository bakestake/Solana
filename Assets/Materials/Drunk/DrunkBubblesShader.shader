Shader "Custom/DrunkEffect_BubbleDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BubbleCount ("Bubble Count", Range(1, 10)) = 3
        _BubbleSize ("Bubble Size", Range(0.01, 0.5)) = 0.2
        _BubbleStrength ("Bubble Strength", Range(0, 0.1)) = 0.02
        _EffectWeight ("Effect Weight", Range(0, 1)) = 0.5

        _ChromaticAberration ("Chromatic Aberration", Range(0, 1)) = 0.1
        _AberrationSpeed ("Aberration Speed", Range(0, 10)) = 0.5

        _DoubleVisionStrength ("Double Vision Strength", Range(0, 0.05)) = 0.01
        _DoubleVisionAngle ("Double Vision Angle (Degrees)", Range(0, 360)) = 45.0
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _BubbleSize;
            float _BubbleStrength;
            int _BubbleCount;
            float _EffectWeight;

            float _ChromaticAberration;
            float _AberrationSpeed;

            float _DoubleVisionStrength;
            float _DoubleVisionAngle;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 BubbleDistortion(float2 uv, float time)
            {
                float2 offset = float2(0, 0);

                for (int i = 0; i < 10; i++)
                {
                    if (i >= _BubbleCount) break;

                    // Pseudo-random position
                    float2 center = float2(
                        frac(sin(i * 12.9898) * 43758.5453),
                        frac(sin(i * 78.233) * 43758.5453)
                    );

                    // Animate position
                    center += 0.05 * float2(sin(time + i), cos(time * 1.2 + i));

                    // Animate size/strength
                    float size = _BubbleSize + 0.05 * sin(time * 1.5 + i);
                    float strength = _BubbleStrength + 0.01 * cos(time * 2.0 + i);

                    float2 diff = uv - center;
                    float dist = length(diff);

                    if (dist < size)
                    {
                        float falloff = 1.0 - smoothstep(0.0, size, dist);
                        offset += normalize(diff) * strength * falloff;
                    }
                }

                return uv + offset;
            }

            float2 GetChromaticAberrationOffset(float2 uv, float strength, float speed)
            {
                float angle = _Time.y * speed;
                float2 offset = float2(cos(angle), sin(angle)) * strength;
                return uv + offset;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = BubbleDistortion(i.uv, _Time.y);

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
