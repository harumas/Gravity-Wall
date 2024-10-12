using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setobs : MonoBehaviour
{
    [SerializeField] private TMP_InputField ScreenportText;
    [SerializeField] private TMP_InputField ScreenpasswordText;

    [SerializeField] private TMP_InputField FaceportText;
    [SerializeField] private TMP_InputField FacepasswordText;

    ObsManager obsManager => ObsManager.instance;

    public void SetScreenOBS()
    {
        obsManager.SetScreenOBS(ScreenportText.text, ScreenpasswordText.text);
    }

    public void SetFaceOBS()
    {
        obsManager.SetFaceOBS(FaceportText.text, FacepasswordText.text);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("PlayTestTitle");
    }
    // Start is called before the first frame update

}
