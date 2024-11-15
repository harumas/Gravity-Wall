using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlinkController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;
    [SerializeField] private float blinkTime = 1.0f;
    private float blinkTimer = 0;
    private static readonly int rEyeBlunk = Shader.PropertyToID("_REyeBlink");
    private static readonly int lEyeBlunk = Shader.PropertyToID("_LEyeBlink");
    // Start is called before the first frame update
    void Start()
    {
        blinkTimer = blinkTime;
    }

    // Update is called once per frame
    void Update()
    {
        blinkTimer -= Time.deltaTime;

        if (blinkTimer <= 0)
        {
            float scale = 0;
            DOTween.To(() => scale, (v) => scale = v, 3, 0.15f)
            .SetLoops(2, LoopType.Yoyo)
            .OnUpdate(() =>
            {
                faceMeshRenderer.materials[1].SetFloat(rEyeBlunk, scale);
                faceMeshRenderer.materials[1].SetFloat(lEyeBlunk, scale);
            });

            blinkTimer = Random.Range(blinkTime, blinkTime + 2.0f);
        }
    }
}
