using R3;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.PlayTest.QuestionnaireForm
{
    public class QuestionnaireFormPresenter : MonoBehaviour
    {
        [SerializeField] private QuestionnaireModel questionnaireModel;
        [SerializeField] private QuestionnaireDatabase questionnaireDatabase;
        [SerializeField] private QuestionnairePageViewer questionnairePageViewer;
        [SerializeField] private RadioButtonManager radioButtonManager;

        // Start is called before the first frame update
        void Start()
        {
            questionnaireModel.Initialize(questionnaireDatabase.questionnaireDatas.Count);

            questionnaireModel.State
            .Subscribe(state =>
            {
                if (state == State.ending)
                {
                    int id = questionnaireDatabase.questionnaireDatas.Count - 1;
                    new JSONWriter().AddPlayerData(questionnaireModel.SaveAnswer(id, questionnaireDatabase.questionnaireDatas[id]));
                }

                if (state == State.loadTitle)
                {
                    SceneManager.LoadScene("PlayTestTitle");
                }

                questionnairePageViewer.ChangeWindow((int)state);
            });

            questionnaireModel.QuestionnaireId
            .Subscribe(id =>
            {
                if (id > 0)
                {
                    new JSONWriter().AddPlayerData(questionnaireModel.SaveAnswer(id - 1, questionnaireDatabase.questionnaireDatas[id - 1]));
                }

                questionnaireModel.ResetAnswer();
                radioButtonManager.ClearRadioButton();

                questionnairePageViewer.SpawnItems(questionnaireDatabase.questionnaireDatas[id], SelectItem);
                questionnairePageViewer.ChangeQuestionnaireTitle(questionnaireDatabase.questionnaireDatas[id].title);
                questionnairePageViewer.ChangeQuestionnairedescription(questionnaireDatabase.questionnaireDatas[id].description);
            });
        }

        void SelectItem(string itemName, bool isCheck, QuestionnaireType questionnaireType)
        {
            if (questionnaireType == QuestionnaireType.checkbox)
            {
                questionnaireModel.CheckBoxAnswer(itemName, isCheck);
            }
            else
            {
                questionnaireModel.SetAnswer(itemName);
            }
        }
    }
}