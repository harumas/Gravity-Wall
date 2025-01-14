using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RadialBlurPass : ScriptableRenderPass
{
    private Material _material;
    private readonly RadialBlurParams _parameters;
    private LocalKeyword _keywordUseDither;

    private static readonly int _idIntensity = Shader.PropertyToID("_Intensity");
    private static readonly int _idSampleCountParams = Shader.PropertyToID("_SampleCountParams");
    private static readonly int _idRadialCenter = Shader.PropertyToID("_RadialCenter");

    public RadialBlurPass(RadialBlurParams parameters, Shader shader)
    {
        _parameters = parameters;
        //シェーダーの取得、マテリアルとキーワードの生成。
        //あまり好ましい取得方法ではありません。あくまでもサンプル。
        _material = CoreUtils.CreateEngineMaterial(shader);
        _keywordUseDither = new LocalKeyword(shader, "USE_DITHER");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("Radial Blur");

        //ラディアルブラーのパラメータを設定。
        _material.SetFloat(_idIntensity, _parameters.intensity);
        _material.SetVector(_idSampleCountParams,
            new Vector3(_parameters.sampleCount,
                1f / _parameters.sampleCount,
                2 <= _parameters.sampleCount ? 1f / (_parameters.sampleCount - 1) : 1));
        _material.SetVector(_idRadialCenter, _parameters.radialCenter);
        _material.SetKeyword(_keywordUseDither, _parameters.useDither);

        Blit(cmd, ref renderingData, _material, 0);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        CoreUtils.Destroy(_material);
    }
}