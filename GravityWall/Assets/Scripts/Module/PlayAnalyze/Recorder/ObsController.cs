using UnityEngine;

namespace Module.PlayAnalyze.Recorder
{
    /// <summary>
    /// OBSに接続して、録画・停止を行うクラス
    /// </summary>
    public class ObsController
    {
        private ObsWebSocket obsWebSocket;

        /// <summary>
        /// OBSに接続します
        /// </summary>
        /// <param name="port">OBSのWebsocketポート番号</param>
        /// <param name="password">OBSのWebsocketパスワード</param>
        public void Connect(int port, string password)
        {
            obsWebSocket = new ObsWebSocket();
            obsWebSocket.Connect(port, password);
        }

        /// <summary>
        /// 録画を開始します
        /// </summary>
        /// <param name="fileName">OBSで指定されたフォルダ内に生成するファイル</param>
        public void StartRecording(string fileName)
        {
            if (obsWebSocket == null)
            {
                Debug.LogError("OBSに接続されていないため、録画を開始できませんでした。");
                return;
            }

            var message = new ObsWebSocket.MessageRequest("StartRecord", "1", null);
            obsWebSocket.SendMessage(message);

            var profileData = new ObsWebSocket.ProfileRequestData("Recording", "FilenameFormatting", fileName);
            var profileMessage = new ObsWebSocket.ProfileMessageRequest("SetProfileParameter", "4", profileData);
            obsWebSocket.SendMessage(profileMessage);
            
            Debug.Log("録画を開始しました。");
        }

        /// <summary>
        /// 録画を停止します
        /// </summary>
        public void StopRecording()
        {
            if (obsWebSocket == null)
            {
                Debug.LogError("OBSに接続されていないため、録画を停止できませんでした。");
                return;
            }

            var message = new ObsWebSocket.MessageRequest("StopRecord", "2", null);
            obsWebSocket.SendMessage(message);
            
            Debug.Log("録画を停止しました。");
        }

        /// <summary>
        /// OBSを切断します
        /// </summary>
        public void Close()
        {
            if (obsWebSocket == null)
            {
                Debug.LogError("OBSに接続されていないため、Closeできませんでした。");
                return;
            }

            obsWebSocket.Close();
        }
    }
}