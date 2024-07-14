#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float ComputeDepth(float4 clippos)
{
    #if defined(SHADER_TARGET_GLSL)
    return (clippos.z / clippos.w) * 0.5 + 0.5;
    #else
    return clippos.z / clippos.w;
    #endif
}

void SSRRayTest_float(Texture2D _CameraDepthTexture, SamplerState _CameraDepthSampler, Texture2D _MainTex,
                      SamplerState _MainTexSampler, float3 pos, float3 viewProj, float3 refDir,
                      out float4 col)
{
    int maxRayNum = 100;
    float3 step = 2.0 / maxRayNum * refDir;

    for (int n = 1; n <= maxRayNum; ++n)
    {
        float3 rayPos = pos + step * n;
        float4 vpPos = mul(viewProj, float4(rayPos, 1.0));
        float2 rayUv = vpPos.xy / vpPos.w * 0.5 + 0.5;
        float rayDepth = ComputeDepth(vpPos);
        float gbufferDepth = LOAD_TEXTURE2D_X_LOD(_BlitTexture, pixelCoords, 0);;
        if (rayDepth - gbufferDepth > 0)
        {
            col += _MainTex.Sample(_MainTexSampler, rayUv) * 0.2;
            break;
        }
    }
}
