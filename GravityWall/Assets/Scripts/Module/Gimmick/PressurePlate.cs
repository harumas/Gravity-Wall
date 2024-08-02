using System.Collections;
using System.Collections.Generic;
using Module.Gimmick;
using UnityEngine;

public class PressurePlate : AbstractSwitch
{
    [SerializeField] private List<AbstractGimmickAffected> gimmickAffecteds = new List<AbstractGimmickAffected>();
    public override bool isOn { get => _isOn; protected set => _isOn = value; }
    private bool _isOn;

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
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && !isOn)
        {
            OnSwitch(true);
        }
    }
}
