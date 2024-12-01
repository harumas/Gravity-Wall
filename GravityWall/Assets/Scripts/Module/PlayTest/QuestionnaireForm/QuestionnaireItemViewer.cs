using System;
using TMPro;
using UnityEngine;

namespace Module.PlayTest.QuestionnaireForm
{
    public abstract class QuestionnaireItemViewer : MonoBehaviour
    {
        protected Action<string, bool, QuestionnaireType> interactItemEvent;
        protected string itemName;
        protected QuestionnaireType questionnaireType;
        protected int id;

        public abstract void InteractItem();
        public abstract void InitializeItem();

        public void Initialize(int id, string itemName, QuestionnaireType questionnaireType, Action<string, bool, QuestionnaireType> interactItemEvent)
        {
            this.id = id;
            this.interactItemEvent = interactItemEvent;
            this.itemName = itemName;
            this.questionnaireType = questionnaireType;

            InitializeItem();
        }
    }
}