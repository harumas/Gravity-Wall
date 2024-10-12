using UnityEngine;
using UnityEngine.UI;

namespace Module.PlayTest.QuestionnaireForm
{
    public class QuestionnaireImageSetter : MonoBehaviour
    {
        [SerializeField] private Image questionnaire;

        public void SetImage(Sprite levelImage)
        {
            questionnaire.sprite = levelImage;
        }
    }
}