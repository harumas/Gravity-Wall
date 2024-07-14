Shader "Hidden/Custom/ColorTransition"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _Intensity;
            float _Speed;
            float _Brightness;
            float _Saturation;

            float3 HSVToRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 color = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
                float3 hsv = float3(sin(_Time.y * _Speed) * 0.5 + 0.5, _Saturation, _Brightness);
                float4 nextColor = float4(HSVToRGB(hsv), 1.0);
                return lerp(color, nextColor, _Intensity);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}