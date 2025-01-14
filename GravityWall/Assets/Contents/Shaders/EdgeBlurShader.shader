Shader "Custom/EdgeBlurShader"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Renderpipeline"="UniversalPipeline"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        float4 _BackgroundColor;

        TEXTURE2D(_DepthMap);
        SAMPLER(sampler_DepthMap);

        half3 SampleColor(float2 uv)
        {
            return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearRepeat, uv).xyz;
        }

        float SampleDepthMap(float2 uv)
        {
            half depth = SAMPLE_DEPTH_TEXTURE(_DepthMap, sampler_DepthMap, uv);
            return depth;
        }

        float SampleDepth(float2 uv)
        {
            #if UNITY_REVERSED_Z
            float depth = SampleSceneDepth(uv);
            #else
                    float depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
            #endif
            return depth;
        }
        ENDHLSL

        Pass
        {
            Name "ScaleDepth"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            // ダウンサンプリング時には1ピクセル分ずらした対角線上の4点からサンプリング
            float4 frag(Varyings i) : SV_Target
            {
                float depth = SampleSceneDepth(i.texcoord);
                depth = step(Eps_float(), depth);
                return float4(depth, depth, depth, 1);;
            }
            ENDHLSL
        }

        Pass
        {
            Name "LerpColor"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            // ダウンサンプリング時には1ピクセル分ずらした対角線上の4点からサンプリング
            float4 frag(Varyings i) : SV_Target
            {
                float4 col = 1;
                col.rgb = lerp(_BackgroundColor,SampleColor(i.texcoord), SampleDepthMap(i.texcoord));

                return col;
            }
            ENDHLSL
        }

        //        Pass
        //        {
        //            HLSLPROGRAM
        //            #pragma vertex Vert
        //            #pragma fragment frag
        //
        //
        //            float GetGaussianWeight(float distance);
        //
        //            half4 frag(Varyings input) : SV_Target
        //            {
        //                #if UNITY_REVERSED_Z
        //                half depth = SampleSceneDepth(input.texcoord).x;
        //                #else
        //                    half depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(input.texcoord).x);
        //                #endif
        //
        //                depth = step(Eps_float(), depth);
        //
        //                float2 dir = _Direction * _MainTex_TexelSize.xy;
        //
        //                float4 color = 0;
        //                for (int j = 0; j < _SamplingTexelAmount; j++)
        //                {
        //                    float2 offset = dir * ((j + 1) * _TexelInterval - 1); //_TexelIntervalでサンプリング距離を調整
        //                    float weight = GetGaussianWeight(j + 1); //ウェイトを計算
        //                    color.rgb += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearRepeat, input.texcoord + offset) *
        //                        weight;
        //                    color.rgb += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearRepeat, input.texcoord - offset) *
        //                        weight;
        //                }
        //
        //                return color;
        //            }
        //
        //            inline float GetGaussianWeight(float distance)
        //            {
        //                return exp((-distance * distance) / (2 * _Dispersion * _Dispersion)) / _Dispersion;
        //            }
        //            ENDHLSL
        //        }
    }
}