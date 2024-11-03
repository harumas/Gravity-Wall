using System.Collections.Generic;

namespace Module.PlayTest.QuestionnaireForm
{
    [System.Serializable]
    public class QuestionnaireEntity
    {
        public int id;
        public string time;
        public string question;
        public List<string> selectedItems;

        public QuestionnaireEntity(int id, string time, string question, List<string> selectedItems)
        {
            this.id = id;
            this.time = time;
            this.question = question;
            this.selectedItems = selectedItems;
        }
    }

    [System.Serializable]
    public class QuestionnaireEntities
    {
        public List<QuestionnaireEntity> questionnaireEntities = new List<QuestionnaireEntity>();
    }
}