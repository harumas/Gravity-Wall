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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            TEXTURE2D_X(_CameraGBufferTexture0);
            SAMPLER(sampler_CameraGBufferTexture0);

            float _Intensity;

            float3 HSVToRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float ComputeDepth(float4 spos)
            {
                #if defined(UNITY_UV_STARTS_AT_TOP)
                return (spos.z / spos.w);
                #else
                    return (spos.z / spos.w) * 0.5 + 0.5;
                #endif
            }

            float SampleDepth(float2 uv)
            {
                #if UNITY_REVERSED_Z
                return SampleSceneDepth(uv);
                #else
                    return lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
                #endif
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float4 col = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv);
                float2 spos = uv * 2.0 - 1.0;

                #if UNITY_UV_STARTS_AT_TOP
                spos.y = -spos.y;
                #endif

                float depth = SampleDepth(uv);
                if (depth == 0) return col;

                //深度のデバッグ
                //return float4(lerp(float3(0, 0, 0), float3(1, 1, 1), depth), 1);

                float4 pos = mul(UNITY_MATRIX_I_VP, float4(spos, depth, 1.0));
                float3 worldPos = pos.xyz / pos.w;

                //ワールド座標のデバッグ
                //return float4(worldPos, 1);

                float3 camDir = normalize(worldPos - _WorldSpaceCameraPos);
                float3 normal = SampleSceneNormals(uv);

                //法線のデバッグ
                //return float4(normal, 1);

                float3 reflectDir = reflect(camDir, normal);

                //反射ベクトルのデバッグ
                return float4(reflectDir, 1);

                int maxRayNum = 200;
                float3 step = 2.0 / maxRayNum * reflectDir;
                float maxThickness = 0.3 / maxRayNum;

                for (int n = 1; n <= maxRayNum; ++n)
                {
                    float3 rayPos = worldPos + step * n;
                    float4 vpPos = mul(UNITY_MATRIX_VP, float4(rayPos, 1.0));
                    float2 rayUv = vpPos.xy / vpPos.w * 0.5 + 0.5;

                    float rayDepth = ComputeDepth(vpPos);
                    float gbufferDepth = SampleDepth(rayUv);

                    //if (rayDepth - gbufferDepth > 0)
                    if (rayDepth - gbufferDepth > 0 && rayDepth - gbufferDepth < maxThickness)
                    {
                        col += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, rayUv) * 0.3;
                        break;
                    }
                }

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}