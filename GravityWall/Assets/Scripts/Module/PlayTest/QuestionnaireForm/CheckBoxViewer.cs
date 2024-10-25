using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.PlayTest.QuestionnaireForm
{
    public class CheckBoxViewer : QuestionnaireItemViewer
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Toggle toggle;

        public override void InitializeItem()
        {
            text.text = itemName;
        }

        public override void InteractItem()
        {
            interactItemEvent.Invoke(itemName, toggle.isOn, questionnaireType);
        }
    }
}