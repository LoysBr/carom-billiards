using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string m_menuSceneName;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void OnMainMenuButtonClick()
    {
        SceneManager.LoadScene(m_menuSceneName);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }
}
