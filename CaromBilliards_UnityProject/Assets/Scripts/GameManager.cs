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

    #region Events
    public delegate void EndOfShot(bool _succeed);
    public event EndOfShot EndOfShotEvent;
    #endregion

    public CameraManager m_cameraManager;

    [HideInInspector]
    public GameState CurrentGameState { get { return m_currentGameSate; } }
    private GameState m_currentGameSate;

    private Vector3 m_shotAimDirection;

    public enum GameState
    {
        WAITING_FOR_SHOT,
        SHOT_IN_PROGRESS,
        END_OF_SHOT,
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

    public void Update()
    {
        //let's check if 3 balls are stoped to know when shot is finished
        if(m_currentGameSate == GameState.SHOT_IN_PROGRESS)
        {
            if(m_whiteBall.IsStopped() && m_yellowBall.IsStopped() && m_redBall.IsStopped())
            {
                SwitchGameState(GameState.END_OF_SHOT);
            }
        }
    }

    public void SwitchGameState(GameState _state)
    {
        m_currentGameSate = _state;
        switch (m_currentGameSate)
        {
            case GameState.WAITING_FOR_SHOT:
                break;
            case GameState.SHOT_IN_PROGRESS:
                break;
            case GameState.END_OF_SHOT:                
                EndOfShotEvent?.Invoke(m_whiteBall.HasCollidedWithTwoOtherBalls());
                m_whiteBall.ResetLastShotCollisions();
                m_yellowBall.ResetLastShotCollisions();
                m_redBall.ResetLastShotCollisions();
                break;
            default:
                break;
        }
    }


    public void OnPlayerShot(float _power)
    {
        SwitchGameState(GameState.SHOT_IN_PROGRESS);

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