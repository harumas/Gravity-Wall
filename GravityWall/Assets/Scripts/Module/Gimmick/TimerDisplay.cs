using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Module.Gimmick
{
    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TimerGate timerGate;

        // Update is called once per frame
        void Update()
        {
            text.text = timerGate.GetTime().ToString("F2");
        }
    }
}