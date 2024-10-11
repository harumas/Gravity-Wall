using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Module.Gimmick;
using UnityEngine;

public class PressurePlate : AbstractSwitch
{
    [SerializeField] private List<AbstractGimmickAffected> gimmickAffecteds = new List<AbstractGimmickAffected>();
    [SerializeField, Tag] private string[] targetTags;
    [SerializeField] private MeshRenderer meshRenderer;
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

        foreach (var gimmick in gimmickAffecteds)
        {
            gimmick.Affect(this);
        }

        // Emissionの色を変更
        meshRenderer.material.SetFloat("_PushRatio", isOn ? 1.0f : 0.0f);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (targetTags.Any(tag => collider.CompareTag(tag)) && !isOn)
        {
            OnSwitch(true);
        }
    }
}
