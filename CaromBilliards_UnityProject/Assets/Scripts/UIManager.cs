using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image        m_shotPowerImage;
    public GameObject   m_shotPowerGroup;
    public Text         m_scoreText;

    void Start()
    {
        m_shotPowerGroup.SetActive(false);

        if (InputManager.Instance)
        {
            InputManager.Instance.InputShotEvent += OnShot;
            InputManager.Instance.InputShotHoldEvent += OnShotPowerChanged;
        }
        if (GameManager.Instance)
            GameManager.Instance.EndOfShotEvent += OnEndOfShot;
        if (ScoreManager.Instance)
            ScoreManager.Instance.CurrentGameScoreChangedEvent += OnScoreChanged;
    }

    private void OnScoreChanged(int _score)
    {
        //TODO StringBuilder
        m_scoreText.text = "Score: " + _score.ToString();
    }

    public void OnShotPowerChanged(float _power)
    {
        m_shotPowerGroup.SetActive(true);
        m_shotPowerImage.transform.localScale = new Vector3(_power, 1, 1);
    }

    public void OnShot(float _power)
    {
        m_shotPowerGroup.SetActive(false);  
    }

    public void OnEndOfShot(bool _succeed)
    {
        if(_succeed)
        {
            //Debug.Log("Super, poiiiiint");
        }
        else
        {
            //Debug.Log("Fail!");
        }

        if(GameManager.Instance)
            GameManager.Instance.SwitchGameState(GameManager.GameState.WAITING_FOR_SHOT);
    }
}
