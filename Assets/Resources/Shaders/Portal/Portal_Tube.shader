Shader "Portal/Tube"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry-1" }
        ZWrite Off
        ColorMask 0

        Stencil
        {
            Ref 2
            Comp Always
            Pass Replace
        }

        Pass
        {
            
        }
    }
}
