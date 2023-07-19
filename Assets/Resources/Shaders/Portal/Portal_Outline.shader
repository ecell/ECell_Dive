Shader "Portal/Outline"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ERadius("External Radius", Range(0, 0.5)) = 0.5
        _IRadius("Internal Radius", Range(0, 0.5)) = 0.4
        _ESmooth("External Smooth", Range(0, 0.5)) = 0.01
        _ISmooth("Internal Smooth", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1"}
        Blend SrcAlpha OneMinusSrcAlpha

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

            float4 _Color;
            float _ERadius;
            float _IRadius;
            float _ESmooth;
            float _ISmooth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }

            float circle(float2 _p, float2 _center, float _radius, float _smooth)
            {
                float c = length(_p - _center);
                return smoothstep(c - _smooth, c + _smooth, _radius);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float big = circle(i.uv, 0.5, _ERadius, _ESmooth);
                float small = circle(i.uv, 0.5, _IRadius, _ISmooth);
                //return fixed4(small.xxxx);
                return fixed4(_Color * saturate(big-small).xxxx);
            }
            ENDCG
        }
    }
}
