using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string m_SceneToLaunch;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void OnNewGameButtonClick()
    {
        SceneManager.LoadScene(m_SceneToLaunch);
    }
}
