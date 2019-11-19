using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string m_menuSceneName;

    #region Singleton
    private static GameManager m_instance;
    public static GameManager Instance
    {
        get
        {
            return m_instance;
        }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }
    #endregion

    [HideInInspector]
    public GameState CurrentGameState { get { return m_currentGameSate; } }
    private GameState m_currentGameSate;

    public enum GameState
    {
        WAITING_FOR_SHOT,
    }
      

    void Start()
    {
        m_currentGameSate = GameState.WAITING_FOR_SHOT;

        InputManager.Instance.InputShotEvent += OnPlayerShot;
    }


    void Update()
    {
        
    }

    public void OnPlayerShot(float _power)
    {
        Debug.Log("OnPlayerShot");
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
