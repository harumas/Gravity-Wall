using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Module.PlayTest.QuestionnaireForm
{
    public class QuestionnairePageViewer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Transform content;
        [SerializeField] private QuestionnaireItemViewer comment, radioButton, radioImageButton, checkBox;
        [SerializeField] private GameObject[] questionnaireViews;
        [SerializeField] private Sprite[] sprite;

        public void SpawnItems(QuestionnaireData questionnaireData, Action<string, bool, QuestionnaireType> interactItemEvent)
        {
            //前の選択肢を全部削除
            foreach (Transform n in content)
            {
                Destroy(n.gameObject);
            }

            int id = 0;
            foreach (var itemName in questionnaireData.selectItemNames)
            {
                var item = Instantiate(GetItemType(questionnaireData.questionnaireType), content);
                item.Initialize(id, itemName, questionnaireData.questionnaireType, interactItemEvent);
                if (questionnaireData.questionnaireType == QuestionnaireType.radioLevelImageButton)
                {
                    item.GetComponent<QuestionnaireImageSetter>().SetImage(sprite[id]);
                }

                id++;
            }
        }

        QuestionnaireItemViewer GetItemType(QuestionnaireType type)
        {
            switch (type)
            {
                case QuestionnaireType.comment:
                    return comment;
                case QuestionnaireType.radioButton:
                    return radioButton;
                case QuestionnaireType.radioLevelImageButton:
                    return radioImageButton;
                case QuestionnaireType.checkbox:
                    return checkBox;
            }
            return comment;
        }

        public void ChangeWindow(int id)
        {
            for (int i = 0; i < questionnaireViews.Count(); i++)
            {
                questionnaireViews[i].SetActive(i == id);
            }
        }

        public void ChangeQuestionnaireTitle(string title)
        {
            titleText.text = title;
        }

        public void ChangeQuestionnairedescription(string description)
        {
            descriptionText.text = description;
        }
    }
}