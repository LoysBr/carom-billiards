using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Persistent Singleton to share score across every scenes and
/// also serialize / deserialize last game score
/// </summary>
public class ScoreManager : MonoBehaviour
{
    #region PersistentSingleton
    private static ScoreManager m_instance;
    public static ScoreManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    
    void Awake()
    {
        if (m_instance == null)     //first opening of menu
        {
            m_instance = this;
        }
        else                       //another scene is loaded
        {
            if (m_instance != this) //we only want to keep the original object
            {
                Destroy(this.gameObject);
            }
        }

        DontDestroyOnLoad(this);
    }
    #endregion

    private int m_currentGameScore;

    #region Events
    public delegate void CurrentGameScoreChanged(int _score);
    public event CurrentGameScoreChanged CurrentGameScoreChangedEvent;
    #endregion

    void Start()
    {
        if(GameManager.Instance) //perhaps we're in another scene, without GameManager
            GameManager.Instance.EndOfShotEvent += OnEndOfShot;

        ResetScores();
    }

    public void ResetScores()
    {
        m_currentGameScore = 0;
    }
    
    private void OnEndOfShot(bool _succeed)
    {
        if (_succeed)
        {
            Debug.Log("Super, poiiiiint");
            m_currentGameScore++;
            CurrentGameScoreChangedEvent?.Invoke(m_currentGameScore);
        }
        else
        {
            Debug.Log("Fail!");
        }

        GameManager.Instance.SwitchGameState(GameManager.GameState.WAITING_FOR_SHOT);
    }
}
