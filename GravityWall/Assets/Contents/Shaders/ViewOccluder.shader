Shader "Unlit/ViewOccluder"
{
    Properties
    {
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
    }

    SubShader
    {

        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "LightMode" = "UniversalForward"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
            }

            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _BaseColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _BaseColor;
            }
            ENDCG
        }
    }
}