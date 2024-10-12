using Module.PlayTest.WebCamera;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObsManager : MonoBehaviour
{
    private ObsController ScreenobsController;
    private ObsController FaceobsController;
    [SerializeField] private string ScreenfileName;
    [SerializeField] private string FacefileName;
    public static ObsManager instance;




    private void Start()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        ScreenobsController = new ObsController();
        FaceobsController = new ObsController();
    }
    public void SetScreenOBS(string port,string password)
    {
        ScreenobsController.Connect(int.Parse(port), password);
    }

    public void SetFaceOBS(string port, string password)
    {
        FaceobsController.Connect(int.Parse(port), password);
    }
    public void StartScreenOBS()
    {
        ScreenobsController.StartRecording(ScreenfileName);
    }

    public void StartOBS()
    {
        FaceobsController.StartRecording(FacefileName);
    }

   public void StopOBS()
    {
        ScreenobsController.StopRecording();
        FaceobsController.StopRecording();
    }
    public void FinishFaceOBS()
    {
        ScreenobsController.StopRecording();
        FaceobsController.StopRecording();
    }

    private void OnApplicationQuit()
    {
        ScreenobsController?.Close();
        FaceobsController?.Close();
    }
    private void OnDestroy()
    {
      //  ScreenobsController?.Close();
    }
}
