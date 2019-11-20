using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string m_menuSceneName;

    public Ball m_whiteBall;
    public Ball m_redBall;
    public Ball m_yellowBall;

    public float m_ballsMaxSpeed; //in m/s

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

    public CameraManager m_cameraManager;

    [HideInInspector]
    public GameState CurrentGameState { get { return m_currentGameSate; } }
    private GameState m_currentGameSate;

    private Vector3 m_shotAimDirection;

    public enum GameState
    {
        WAITING_FOR_SHOT,
    }
      

    void Start()
    {
        m_currentGameSate = GameState.WAITING_FOR_SHOT;

        InputManager.Instance.InputShotEvent += OnPlayerShot;

        m_cameraManager.OnCameraAimDirectionChanged += OnAimDirectionChanged;
        m_cameraManager.RefreshCameraPosition();
        m_cameraManager.RefreshCameraOrientation();

        m_whiteBall.m_maxSpeed = m_yellowBall.m_maxSpeed = m_redBall.m_maxSpeed = m_ballsMaxSpeed;        
    }

    

    void Update()
    {
        
    }

    public void OnPlayerShot(float _power)
    {
        //Debug.Log("OnPlayerShot");

        m_whiteBall.OnBallShot(_power, m_shotAimDirection);
    }

    public void OnAimDirectionChanged(Vector3 _direction)
    {
        m_shotAimDirection = _direction;
    }

    public void ResetBallPos()
    {
        m_whiteBall.transform.position = new Vector3(-0.599f, 1.43075f, 99.044f);
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
