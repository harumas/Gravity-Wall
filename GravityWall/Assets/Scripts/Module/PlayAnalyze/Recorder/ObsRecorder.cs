using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.PlayAnalyze.Recorder
{
    /// <summary>
    /// OBSの録画をテストする画面
    /// </summary>
    public class ObsRecorder : MonoBehaviour
    {
        private ObsController obsController;
        [SerializeField] private string fileName;
        [SerializeField] private Button connectButton;
        [SerializeField] private TMP_InputField portText;
        [SerializeField] private TMP_InputField passwordText;
        [SerializeField] private Button startRecordButton;
        [SerializeField] private Button stopRecordButton;

        private void Start()
        {
            obsController = new ObsController();
        
            connectButton.onClick.AddListener(() => { obsController.Connect(int.Parse(portText.text), passwordText.text); });
            startRecordButton.onClick.AddListener(() => { obsController.StartRecording(fileName); });
            stopRecordButton.onClick.AddListener(() => { obsController.StopRecording(); });
        }

        private void OnDestroy()
        {
            obsController?.Close();
        }
    }
}