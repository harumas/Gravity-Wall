using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Module.Gimmick;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : AbstractSwitch
{
    [SerializeField] private List<AbstractGimmickAffected> gimmickAffecteds = new List<AbstractGimmickAffected>();
    [SerializeField, Tag] private string[] targetTags;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private UnityEvent onEvent;
    public override bool isOn { get => _isOn; protected set => _isOn = value; }
    private bool _isOn;
    private float intensity = 8.0f;

    void Start()
    {
        isOn = false;
    }

    public override void OnSwitch(bool isOn)
    {
        this.isOn = isOn;

        if (isOn)
        {
            onEvent.Invoke();
        }

        foreach (var gimmick in gimmickAffecteds)
        {
            gimmick.Affect(this);
        }

        meshRenderer.material.EnableKeyword("_EMISSION");

        // 新しいEmissionカラーを設定 (例: 赤色)
        Color emissionColor = isOn ? Color.green : Color.yellow;

        // Emissionの色を変更
        meshRenderer.material.SetColor("_EmissionColor", emissionColor * intensity);

        Debug.Log("OnSwitch");
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (targetTags.Any(tag => collider.CompareTag(tag)) && !isOn)
        {
            OnSwitch(true);
        }
    }
}
