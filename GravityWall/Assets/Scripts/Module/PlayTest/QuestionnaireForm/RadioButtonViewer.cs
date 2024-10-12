using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.PlayTest.QuestionnaireForm
{
    public class RadioButtonViewer : QuestionnaireItemViewer
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Toggle toggle;
        private RadioButtonManager radioButtonManager;

        public override void InteractItem()
        {
            if (!toggle.isOn) return;

            interactItemEvent.Invoke(itemName, toggle.isOn, questionnaireType);
            radioButtonManager.IntaractButton(id);
        }

        public override void InitializeItem()
        {
            text.text = itemName;
            radioButtonManager = transform.parent.GetComponent<RadioButtonManager>();
            radioButtonManager.SetRadioButton(this);
        }
    }
}