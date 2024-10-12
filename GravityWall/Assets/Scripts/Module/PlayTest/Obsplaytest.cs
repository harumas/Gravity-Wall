using Module.PlayTest.WebCamera;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Obsplaytest : MonoBehaviour
{
    [SerializeField] private GameObject Button;
    [SerializeField] private GameObject Button1;
    private ObsManager obsManager => ObsManager.instance;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(Button);
        EventSystem.current.SetSelectedGameObject(Button1);
        EventSystem.current.firstSelectedGameObject = Button;
    }


    public void OnFace()
    {
        obsManager.StartOBS();
        SceneManager.LoadScene("Tutorial01");
    }
    
    public void OffFace()
    {
        obsManager.StartScreenOBS();
        SceneManager.LoadScene("Tutorial01");
    }
   
}
