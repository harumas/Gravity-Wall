using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ラディアルブラーのシェーダーで用いるパラメータ。
/// </summary>
[Serializable]
public class RadialBlurParams
{
    [Range(0, 1), Tooltip("ブラーの強さ")] public float intensity = 0.4f;
    [Min(1), Tooltip("サンプリング回数")] public int sampleCount = 3;
    [Tooltip("エフェクトの中心")] public Vector2 radialCenter = new Vector2(0.5f, 0.5f);
    [Tooltip("ディザリングを利用する")] public bool useDither = true;
}

public class RadialBlurFeature : ScriptableRendererFeature
{
    [SerializeField] private RadialBlurParams _parameters;
    [SerializeField] private Shader radialBlurShader;
    [SerializeField] private RenderPassEvent _renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    private RadialBlurPass _pass;

    public override void Create()
    {
        _pass = new RadialBlurPass(_parameters, radialBlurShader)
        {
            renderPassEvent = _renderPassEvent,
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass != null) renderer.EnqueuePass(_pass);
    }

    public void OnDestroy() => _pass?.Dispose();
}