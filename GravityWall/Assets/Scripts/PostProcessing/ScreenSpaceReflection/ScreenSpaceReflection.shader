Shader "Hidden/Custom/ScreenSpaceReflection"
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
            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D_X(_CameraGBufferTexture2);
            SAMPLER(sampler_CameraGBufferTexture2);

            float _Intensity;
            float4x4 _InvViewProj;
            float4x4 _ViewProj;

            float3 HSVToRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float4 col = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv);

                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                if (depth >= 1.0) return col;

                float2 spos = 2.0 * uv - 1.0;
                float4 pos = mul(UNITY_MATRIX_I_VP, float4(spos, depth, 1.0));
                pos = pos / pos.w;

                float3 camDir = normalize(pos - _WorldSpaceCameraPos);
                float3 normal = SAMPLE_TEXTURE2D_X(_CameraGBufferTexture2, sampler_CameraGBufferTexture2, uv) * 2.0 - 1.0;
                float3 refDir = normalize(camDir - 2.0 * dot(camDir, normal) * normal);

                return float4(refDir, 1);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}