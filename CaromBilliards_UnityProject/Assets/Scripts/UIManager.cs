using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image        m_shotPowerImage;
    public GameObject   m_shotPowerGroup;
    public GameObject   m_replayButton;

    public GameObject   m_scorePanel;
    public Text         m_scoreText;
    public Text         m_elapsedTimeText;
    public Text         m_shotNumberText;

    void Start()
    {
        m_shotPowerGroup.SetActive(false);

        if (InputManager.Instance)
        {
            InputManager.Instance.InputShotEvent += OnShot;
            InputManager.Instance.InputShotHoldEvent += OnShotPowerChanged;
        }
        if (GameManager.Instance)
        {
            GameManager.Instance.SwitchStateEvent += OnSwitchGameStateEvent;
        }

        m_replayButton.SetActive(false);
    }

    private void Update()
    {
        if(ScoreManager.Instance)
        {
            m_scoreText.text = "Score : " + ScoreManager.Instance.CurrentGameScore;
            int sec = ((int)ScoreManager.Instance.ElapsedTime) % 60;
            if(sec >= 10)
                m_elapsedTimeText.text = ((int) (ScoreManager.Instance.ElapsedTime / 60)).ToString() + ":" + sec.ToString();
            else
                m_elapsedTimeText.text = ((int) (ScoreManager.Instance.ElapsedTime / 60)).ToString() + ":0" + sec.ToString();

            m_shotNumberText.text = "Shots : " + ScoreManager.Instance.ShotNumber;
        }
        else
        {
            m_scorePanel.SetActive(false);
        }
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

    public void OnMainMenuButtonClicked()
    {
        if (GameManager.Instance)
            GameManager.Instance.BackToMainMenu();
    }

    public void OnQuitButtonClicked()
    {
        if (GameManager.Instance)
            GameManager.Instance.Quit();
    }

    public void OnReplayButtonClicked()
    {
        if (GameManager.Instance)
            GameManager.Instance.ReplayStart();
    }

    public void OnSwitchGameStateEvent(GameManager.GameState _state)
    {
        switch (_state)
        {
            case GameManager.GameState.WAITING_FOR_SHOT:
                break;
            case GameManager.GameState.SHOT_IN_PROGRESS:
                m_replayButton.SetActive(false);
                break;
            case GameManager.GameState.END_OF_SHOT:
                m_replayButton.SetActive(true);
                break;
            case GameManager.GameState.REPLAY_IN_PROGRESS:
                break;
            default:
                break;
        }
    }
}