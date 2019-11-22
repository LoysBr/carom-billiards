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
            DontDestroyOnLoad(this);
        }
        else                       //another scene is loaded
        {
            if (m_instance != this) //we only want to keep the original object
            {                
                DestroyImmediate(this.gameObject);
            }
        }        
    }
    #endregion

    private int     m_currentGameScore;
    public int      CurrentGameScore { get { return m_currentGameScore; } }
    private float   m_elapsedTime;
    public float    ElapsedTime { get { return m_elapsedTime; } }
    private int     m_shotNumber;
    public int      ShotNumber { get { return m_shotNumber; } }
    

    public void Start()
    {
        if (GameManager.Instance) 
            GameManager.Instance.EndOfShotEvent += OnEndOfShot;
    }

    void OnLevelWasLoaded()
    {
        if (GameManager.Instance) 
            GameManager.Instance.EndOfShotEvent += OnEndOfShot;

        ResetScores();
    }

    void Update()
    {
        m_elapsedTime += Time.deltaTime;    
    }

    public void ResetScores()
    {
        m_currentGameScore = 0;
        m_elapsedTime = 0f;
        m_shotNumber = 0;
    }
    
    private void OnEndOfShot(bool _succeed)
    {
        if (_succeed)
        {
            Debug.Log("Super, poiiiiint");
            m_currentGameScore++;
        }
        else
        {
            Debug.Log("Fail!");
        }

        m_shotNumber++;
    }
}
