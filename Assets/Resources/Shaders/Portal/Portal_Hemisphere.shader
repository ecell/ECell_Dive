Shader "Portal/Portal_Hemisphere"
{
    Properties
    {
        _Tex1 ("Texture 1", 2D) = "white" {}
        _Tex1Speed("Tex1Speed", float) = 0.05
        _Color("Texture Color", Color) = (1, 1, 1, 1)

        _BgColor("Void Color", Color) = (1, 1, 1, 1)
        _BgRadius("Void Radius", float) = 0.1
        _BgSmoothness("Void Smoothness", float) = 0.5

        _Resolution ("Noise Resolution", Vector) = (0, 0, 0, 0)
        _Scale("Noise Scale", float) = 5
        _ScrollingSpeed("Noise Scrolling Speed", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Front

        Stencil
        {
            Ref 2 // StencilRef
            Comp NotEqual
            Pass Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define UI0 1597334673U
            #define UI1 3812015801U
            #define UI2 uint2(UI0, UI1)
            #define UIF (1.0 / float(0xffffffffU))

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

            sampler2D _Tex1;
            float4 _Tex1_ST;
            float _Tex1Speed;
            float4 _Color;

            float4 _BgColor;
            float _BgRadius;
            float _BgSmoothness;

            float2 _Resolution;
            float _Scale;
            float _ScrollingSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Tex1);
                return o;
            }

            fixed3 smoothStepMask(float _u)
            {
                float edge = 0.65;
                float smooth = 0.1;
                return smoothstep(_u - smooth, _u + smooth, edge);
            }

            // 2D Random
            //https://www.shadertoy.com/view/XdGfRR
            float hash12(float2 p)
            {
                uint2 q = uint2(int2(p)) * UI2;
                uint n = (q.x ^ q.y) * UI0;
                return float(n) * UIF;
            }

            float2 randomGradient(in float2 _pos)
            {
                float rad = radians(hash12(_pos) * 360);
                return fixed2(cos(rad), sin(rad));
            }

            float gradientNoise(in float2 _pos)
            {
                float2 ip = floor(_pos);
                float2 fp = frac(_pos);

                //smooth interpolation of the fractional part
                fixed2 smInt = smoothstep(0., 1., fp);

                //four corners in 2D of a tile
                float2 d00 = randomGradient(ip.xy);
                float2 d10 = randomGradient(ip + fixed2(1.0, 0.0));
                float2 d01 = randomGradient(ip + fixed2(0.0, 1.0));
                float2 d11 = randomGradient(ip + fixed2(1.0, 1.0));

                return lerp(
                    lerp(dot(d00, fp), dot(d10, fp-fixed2(1.0, 0.0)), smInt.x),
                    lerp(dot(d01, fp-fixed2(0.0, 1.0)), dot(d11, fp-fixed2(1.0, 1.0)), smInt.x),
                    smInt.y);
                
            }

            float circle(float2 _p, float2 _center, float _radius, float _smooth)
            {
                float c = length(_p - _center);
                return smoothstep(c - _smooth, c + _smooth, _radius);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //Portal depth Mask
                //fixed3 depthMask = smoothStepMask(1-i.uv.y);
                fixed depthMask = 1-circle(i.uv, float2(0.5,1), _BgRadius, _BgSmoothness);

                fixed2 pos = (i.uv/_Resolution)*_Scale - _Time.xy * _ScrollingSpeed;
                float n = gradientNoise(pos);
                n = 0.5 + 0.5*n;

                // sample the texture
                fixed4 texSample = tex2D(_Tex1, n + _Tex1Speed * _Time.y);
                //return fixed4(n.xxx, 1);
                return fixed4(_Color * texSample.xyz * depthMask + _BgColor * (1-depthMask), 1);
            }
            ENDCG
        }
    }
}
