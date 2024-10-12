using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Module.PlayTest.QuestionnaireForm
{
    public class CommentViewer : QuestionnaireItemViewer
    {
        [SerializeField] private TextMeshProUGUI placeholder;
        [SerializeField] private TMP_InputField inputField;
        public override void InteractItem()
        {
            interactItemEvent.Invoke(inputField.text, true, questionnaireType);
        }

        public override void InitializeItem()
        {
            placeholder.text = itemName;
        }
    }
}