using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Module.PlayTest.QuestionnaireForm
{
    public enum State
    {
        opening,
        questionnaire,
        ending,
        loadTitle,
    }

    public class QuestionnaireModel : MonoBehaviour
    {
        public ReadOnlyReactiveProperty<int> QuestionnaireId => questionnaireId;
        private readonly ReactiveProperty<int> questionnaireId = new ReactiveProperty<int>();

        public List<string> answers { get; private set; }

        public ReadOnlyReactiveProperty<State> State => state;
        private readonly ReactiveProperty<State> state = new ReactiveProperty<State>();

        private int maxQuestionnaireCount;
        public void Initialize(int maxQuestionnaireCount)
        {
            this.maxQuestionnaireCount = maxQuestionnaireCount;
            answers = new List<string>();
        }

        public void ResetAnswer()
        {
            answers.Clear();
        }

        public void SetAnswer(string answer)
        {
            if (answers.Count <= 0)
            {
                answers.Add(answer);
                return;
            }

            answers[0] = answer;
        }

        public void CheckBoxAnswer(string answer, bool isCheck)
        {
            if (isCheck)
            {
                answers.Add(answer);
            }
            else
            {
                answers.Remove(answer);
            }
        }

        public void NextQuestionnaire()
        {
            if (state.Value == QuestionnaireForm.State.opening)
            {
                state.Value++;
                return;
            }

            if (questionnaireId.Value >= maxQuestionnaireCount - 1)
            {
                state.Value++;
                return;
            }

            questionnaireId.Value++;
        }

        public QuestionnaireEntity SaveAnswer(int id, QuestionnaireData questionnaireData)
        {
            QuestionnaireEntity questionnaireEntity = new QuestionnaireEntity(id, DateTime.Now.ToString(), questionnaireData.description, answers);
            return questionnaireEntity;
        }

        public void BackQuestionnaire()
        {
            if (questionnaireId.Value <= 0) return;
            questionnaireId.Value--;
        }
    }
}