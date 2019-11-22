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

    public float    m_ballsMaxSpeed; //in m/s
    public float    m_endOfShotWaitDuration;
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

    public delegate void EndOfGame();
    public event EndOfGame EndOfGameEvent;
    #endregion

    public CameraManager m_cameraManager;

    #region GameState
    [HideInInspector]
    public GameState CurrentGameState { get { return m_currentGameSate; } }
    private GameState m_currentGameSate;   

    public enum GameState
    {
        WAITING_FOR_SHOT,
        SHOT_IN_PROGRESS,
        END_OF_SHOT,
        REPLAY_IN_PROGRESS,
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
        m_currentGameSate = GameState.WAITING_FOR_SHOT;

        if (InputManager.Instance)
            InputManager.Instance.InputShotEvent += OnPlayerShot;

        m_cameraManager.CameraChangedAimDirectionEvent += OnAimDirectionChanged;
        m_cameraManager.RefreshCameraPosition();
        m_cameraManager.RefreshCameraOrientation();

        m_whiteBall.m_maxSpeed = m_yellowBall.m_maxSpeed = m_redBall.m_maxSpeed = m_ballsMaxSpeed;
    }

    public void Update()
    {
        switch (m_currentGameSate)
        {
            case GameState.WAITING_FOR_SHOT:
                break;
            case GameState.SHOT_IN_PROGRESS:
                if (m_whiteBall.IsStopped() && m_yellowBall.IsStopped() && m_redBall.IsStopped())
                {
                    SwitchGameState(GameState.END_OF_SHOT);
                }
                break;
            case GameState.END_OF_SHOT:
                m_endOfShotWaitTimer += Time.deltaTime;
                if(m_endOfShotWaitTimer >= m_endOfShotWaitDuration)
                {
                    SwitchGameState(GameState.WAITING_FOR_SHOT);
                }
                break;
            case GameState.REPLAY_IN_PROGRESS:
                if (m_whiteBall.IsStopped() && m_yellowBall.IsStopped() && m_redBall.IsStopped())
                {
                    SwitchGameState(GameState.WAITING_FOR_SHOT);
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
            case GameState.WAITING_FOR_SHOT:
                m_cameraManager.SetCameraPositionWithAngleFromBase(0f);
                break;
            case GameState.SHOT_IN_PROGRESS:
                break;
            case GameState.END_OF_SHOT:                
                EndOfShotEvent?.Invoke(m_whiteBall.HasCollidedWithTwoOtherBalls());
                m_whiteBall.ResetLastShotCollisions();
                m_yellowBall.ResetLastShotCollisions();
                m_redBall.ResetLastShotCollisions();
                m_endOfShotWaitTimer = 0;
                break;
            case GameState.REPLAY_IN_PROGRESS:
                break;
            default:
                break;
        }

        SwitchStateEvent?.Invoke(m_currentGameSate);
    }       

    public void OnPlayerShot(float _power)
    {
        SwitchGameState(GameState.SHOT_IN_PROGRESS);

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

    public void ReplayStart()
    {
        Debug.Log("start replay");

        SwitchGameState(GameState.REPLAY_IN_PROGRESS);
        PlaceElementsLikeBeforeShot();

        //attendre un peu TODO

        m_whiteBall.OnPlayerShot(m_lastShotData.shotPower, m_lastShotData.shotDirection);
    }

    public void FinalizeGame()
    {
        EndOfGameEvent?.Invoke();
    }

    public void ResetBallPos()
    {
        //TODO Change this
        m_whiteBall.transform.position = new Vector3(-0.599f, 1.43075f, 99.044f);
    }      

    public void BackToMainMenu()
    {
        FinalizeGame();
        SceneManager.LoadScene(m_menuSceneName);
    }

    public void Quit()
    {
        FinalizeGame();
        Application.Quit();
    }

}