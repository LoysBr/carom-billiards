using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string m_menuSceneName;

    public Ball     m_whiteBall;
    public Ball     m_redBall;
    public Ball     m_yellowBall;
    private Vector3 m_whiteBallStartPos;
    private Vector3 m_yellowBallStartPos;
    private Vector3 m_redBallStartPos;

    public float    m_ballsMaxSpeed; //in m/s
    public float    m_endOfShotWaitDuration;
    public int      m_gameOverScoreToReach;

    private float   m_endOfShotWaitTimer;

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

    public delegate void SwitchState(GameState _newState);
    public event SwitchState SwitchStateEvent;

    public delegate void GameOver();
    public event GameOver GameOverEvent;
    #endregion

    public CameraManager m_cameraManager;

    #region GameState
    [HideInInspector]
    public GameState CurrentGameState { get { return m_currentGameSate; } }
    private GameState m_currentGameSate;   

    public enum GameState
    {
        Shooting,
        ProcessingShot,
        EndOfShot,
        ProcessingReplay,
        GameOverScreen
    }
    #endregion

    #region ReplayData
    struct ReplayShotData
    {
        public Vector3 whiteBallPos;
        public Vector3 yellowBallPos;
        public Vector3 redBallPos;
        public Vector3 shotDirection;
        public float   shotPower; //0 to 1
        public float   cameraAngleFromBase; //the m_angleOffsetFromBaseDir, used to calculate camera position 
    }

    ReplayShotData m_lastShotData;
    #endregion

    private Vector3 m_shotAimDirection; //the horizontal/flat ball shot direction

    void Start()
    {      
        if (InputManager.Instance)
            InputManager.Instance.InputShotEvent += OnPlayerShot;

        m_cameraManager.CameraChangedAimDirectionEvent += OnAimDirectionChanged;

        m_whiteBall.m_maxSpeed = m_yellowBall.m_maxSpeed = m_redBall.m_maxSpeed = m_ballsMaxSpeed;
        m_whiteBallStartPos = m_whiteBall.transform.position;
        m_yellowBallStartPos = m_yellowBall.transform.position;
        m_redBallStartPos = m_redBall.transform.position;

        SwitchGameState(GameState.Shooting);        
    }

    public void Update()
    {
        switch (m_currentGameSate)
        {
            case GameState.Shooting:
                break;
            case GameState.ProcessingShot:
                if (m_whiteBall.IsStopped() && m_yellowBall.IsStopped() && m_redBall.IsStopped())
                {
                    SwitchGameState(GameState.EndOfShot);
                }
                break;
            case GameState.EndOfShot:
                m_endOfShotWaitTimer += Time.deltaTime;
                if(m_endOfShotWaitTimer >= m_endOfShotWaitDuration)
                {
                    SwitchGameState(GameState.Shooting);
                }
                break;
            case GameState.ProcessingReplay:
                if (m_whiteBall.IsStopped() && m_yellowBall.IsStopped() && m_redBall.IsStopped())
                {
                    SwitchGameState(GameState.Shooting);
                }
                break;
            default:
                break;
        }     
    }

    private void SwitchGameState(GameState _state)
    {
        m_currentGameSate = _state;
        switch (m_currentGameSate)
        {
            case GameState.Shooting:
                m_cameraManager.SetCameraPositionWithAngleFromBase(0f);
                break;
            case GameState.ProcessingShot:
                break;
            case GameState.EndOfShot: 
                EndOfShotEvent?.Invoke(m_whiteBall.HasCollidedWithTwoOtherBalls());
                m_whiteBall.ResetLastShotCollisions();
                m_yellowBall.ResetLastShotCollisions();
                m_redBall.ResetLastShotCollisions();
                m_endOfShotWaitTimer = 0;

                //checking if Game is over
                if (ScoreManager.Instance)
                {
                    if (ScoreManager.Instance.CurrentGameScore >= m_gameOverScoreToReach)
                    {
                        SwitchGameState(GameState.GameOverScreen);
                    }
                }
                else
                {
                    Debug.LogError("Without a ScoreManager there will be no GameOver.");
                }
                break;
            case GameState.ProcessingReplay:
                break;
            case GameState.GameOverScreen:
                GameOverEvent?.Invoke();
                break;
            default:
                break;
        }

        SwitchStateEvent?.Invoke(m_currentGameSate);
    }

    public void RestartGame()
    {
        if (ScoreManager.Instance)
            ScoreManager.Instance.ResetScores();

        ResetBallsPos();

        SwitchGameState(GameState.Shooting);
    }

    public void OnPlayerShot(float _power)
    {
        SwitchGameState(GameState.ProcessingShot);

        m_whiteBall.OnPlayerShot(_power, m_shotAimDirection.normalized); //it's already normalized but it's a double check 

        SaveLastShotData(_power);
    }

    private void SaveLastShotData(float _shotPower)
    {
        //Debug.Log("Save Last Shot Data");
        m_lastShotData = new ReplayShotData();
        m_lastShotData.whiteBallPos = m_whiteBall.gameObject.transform.position;
        m_lastShotData.yellowBallPos = m_yellowBall.gameObject.transform.position;
        m_lastShotData.redBallPos = m_redBall.gameObject.transform.position;
        m_lastShotData.shotDirection = m_shotAimDirection.normalized;
        m_lastShotData.shotPower = _shotPower;
        m_lastShotData.cameraAngleFromBase = m_cameraManager.AimAngleFromBase;
    }

    private void PlaceElementsLikeBeforeShot()
    {
        //Debug.Log("PlaceElementsLikeBeforeShot");
        m_whiteBall.gameObject.transform.position = m_lastShotData.whiteBallPos;
        m_yellowBall.gameObject.transform.position = m_lastShotData.yellowBallPos;
        m_redBall.gameObject.transform.position = m_lastShotData.redBallPos;

        m_cameraManager.SetCameraPositionWithAngleFromBase(m_lastShotData.cameraAngleFromBase);
    }       

    public void OnAimDirectionChanged(Vector3 _direction)
    {
        m_shotAimDirection = _direction;
    }

    public void StartReplay()
    {
        Debug.Log("start replay");

        SwitchGameState(GameState.ProcessingReplay);
        PlaceElementsLikeBeforeShot();

        //attendre un peu TODO

        m_whiteBall.OnPlayerShot(m_lastShotData.shotPower, m_lastShotData.shotDirection);
    }

    public void ResetBallsPos()
    {
        m_whiteBall.transform.position = m_whiteBallStartPos;
        m_yellowBall.transform.position = m_yellowBallStartPos;
        m_redBall.transform.position = m_redBallStartPos;
    }      

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(m_menuSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

}