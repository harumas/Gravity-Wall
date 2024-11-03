using UnityEngine;

namespace Module.PlayTest.WebCamera
{
    public class ObsController
    {
        private ObsWebSocket obsWebSocket;

        public void Connect(int port, string password)
        {
            obsWebSocket = new ObsWebSocket();
            obsWebSocket.Connect(port, password);
        }

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