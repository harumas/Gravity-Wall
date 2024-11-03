using System.IO;
using UnityEngine;

namespace Module.PlayTest.QuestionnaireForm
{
    public class JSONWriter
    {
        private string filePath;

        public void AddPlayerData(QuestionnaireEntity newAnswer)
        {
            filePath = UnityEngine.Application.persistentDataPath + "/playerData.json";

            QuestionnaireEntities playerDataList = LoadPlayerDataList();

            // 新しいプレイヤーデータをリストに追加
            playerDataList.questionnaireEntities.Add(newAnswer);

            // JSONにシリアライズしてファイルに書き込み
            string jsonData = JsonUtility.ToJson(playerDataList, true);
            File.WriteAllText(filePath, jsonData);

            Debug.Log("新しいデータが追加されました: " + filePath);
        }

        private QuestionnaireEntities LoadPlayerDataList()
        {
            // JSONファイルが存在するか確認
            if (File.Exists(filePath))
            {
                // JSONファイルの内容を読み込み
                string jsonData = File.ReadAllText(filePath);

                // デシリアライズしてオブジェクトに変換
                return JsonUtility.FromJson<QuestionnaireEntities>(jsonData);
            }
            else
            {
                // ファイルが存在しない場合、新しいリストを返す
                return new QuestionnaireEntities();
            }
        }
    }
}