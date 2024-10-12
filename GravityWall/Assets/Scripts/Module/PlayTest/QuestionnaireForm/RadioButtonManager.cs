using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Module.PlayTest.QuestionnaireForm
{
    public class RadioButtonManager : MonoBehaviour
    {
        private List<Toggle> buttons = new List<Toggle>();

        public void ClearRadioButton()
        {
            buttons.Clear();
        }

        public void SetRadioButton(RadioButtonViewer radioButton)
        {
            buttons.Add(radioButton.GetComponent<Toggle>());
        }

        public void IntaractButton(int id)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Toggle radioButton = buttons[i];
                if (i == id) continue;
                radioButton.isOn = false;
            }
        }
    }
}