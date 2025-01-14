Shader "Custom/RenderFeature/KawaseBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //   _offset ("Offset", float) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            float SampleSceneDepth(float2 uv)
            {
                half depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                return depth;
            }

            float _Offset;
            float4 _BackgroundColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f input) : SV_Target
            {
                float2 res = _MainTex_TexelSize.xy;
                float i = _Offset;
                const float eps = 0.00001;

                float depth = 0;
                depth += step(eps, SampleSceneDepth(input.uv + float2(i, i) * res));
                depth += step(eps, SampleSceneDepth(input.uv + float2(i, -i) * res));
                depth += step(eps, SampleSceneDepth(input.uv + float2(-i, i) * res));
                depth += step(eps, SampleSceneDepth(input.uv + float2(-i, -i) * res));
                depth += step(eps, SampleSceneDepth(input.uv + float2(0, i) * res));
                depth += step(eps, SampleSceneDepth(input.uv + float2(0, -i) * res));
                depth += step(eps, SampleSceneDepth(input.uv + float2(-i, 0) * res));
                depth += step(eps, SampleSceneDepth(input.uv + float2(i, 0) * res));
                depth /= 8.0f;

                float4 mainColor = tex2D(_MainTex, input.uv);
                 float3 color = lerp(_BackgroundColor, mainColor.xyz, depth);
                 return float4(color, mainColor.a);

                return float4(depth, depth, depth, 1);
            }
            ENDHLSL
        }
    }
}