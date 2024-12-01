using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.PlayTest.QuestionnaireForm
{
    public enum QuestionnaireType
    {
        comment,
        radioButton,
        radioLevelImageButton,
        checkbox,
    }

    [CreateAssetMenu(fileName = "QuestionnaireDatabase", menuName = "ScriptableObjects/CreateQuestionnaireDatabase")]
    public class QuestionnaireDatabase : ScriptableObject
    {
        public List<QuestionnaireData> questionnaireDatas = new List<QuestionnaireData>();
    }

    [Serializable]
    public class QuestionnaireData
    {
        public string title;
        public string description;
        public QuestionnaireType questionnaireType;
        public List<string> selectItemNames = new List<string>();
    }
}