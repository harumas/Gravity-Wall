using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlinkController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;
    [SerializeField] private float blinkTime = 1.0f;

    private float blinkTimer = 0;
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
            DOTween.To(() => scale, (v) => scale = v, 3, 0.3f)
            .SetLoops(2, LoopType.Yoyo)
            .OnUpdate(() =>
            {
                faceMeshRenderer.materials[1].SetFloat("_REyeBlink", scale);
                faceMeshRenderer.materials[1].SetFloat("_LEyeBlink", scale);
            });

            blinkTimer = Random.Range(blinkTime, blinkTime + 2.0f);
        }
    }
}
