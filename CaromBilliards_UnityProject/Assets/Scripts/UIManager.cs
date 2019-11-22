﻿using System;
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

    public GameObject   m_gameOverPanel;
    //I duplicated the score objects for game over panel in case we maybe don't 
    //want the same aspect than for the ingame panel
    public Text         m_gameOverScoreText;
    public Text         m_gameOverElapsedTimeText;
    public Text         m_gameOverShotNumberText;

    void Start()
    {
        InitGameUI();

        if (InputManager.Instance)
        {
            InputManager.Instance.InputShotEvent += OnShot;
            InputManager.Instance.InputShotHoldEvent += OnShotPowerChanged;
        }
        if (GameManager.Instance)
        {
            GameManager.Instance.SwitchStateEvent += OnSwitchGameStateEvent;
            GameManager.Instance.GameOverEvent += OnGameOver;
        }        
    }    

    private void InitGameUI()
    {
        if (m_shotPowerGroup)
            m_shotPowerGroup.SetActive(false);
        if (m_replayButton)
            m_replayButton.SetActive(false);
        if (m_gameOverPanel)
            m_gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (m_scorePanel)
        {
            if (ScoreManager.Instance)
            {
                if (m_scoreText)
                    m_scoreText.text = "Score : " + ScoreManager.Instance.CurrentGameScore;

                if (m_elapsedTimeText)
                {
                    int sec = ((int)ScoreManager.Instance.ElapsedTime) % 60;
                    if (sec >= 10)
                        m_elapsedTimeText.text = "Time : " + ((int)(ScoreManager.Instance.ElapsedTime / 60)).ToString() + ":" + sec.ToString();
                    else
                        m_elapsedTimeText.text = "Time : " + ((int)(ScoreManager.Instance.ElapsedTime / 60)).ToString() + ":0" + sec.ToString();
                }

                if (m_shotNumberText)
                    m_shotNumberText.text = "Shots : " + ScoreManager.Instance.ShotNumber;
            }
            else
            {
                m_scorePanel.SetActive(false);
            }
        }
    }   

    public void OnGameOver()
    {
        if (ScoreManager.Instance && m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);

            if (m_gameOverScoreText)
                m_gameOverScoreText.text = "Score : " + ScoreManager.Instance.CurrentGameScore;

            if (m_gameOverElapsedTimeText)
            {
                int sec = ((int)ScoreManager.Instance.ElapsedTime) % 60;
                if (sec >= 10)
                    m_gameOverElapsedTimeText.text = "Elapsed Time : " + ((int)(ScoreManager.Instance.ElapsedTime / 60)).ToString() + ":" + sec.ToString();
                else
                    m_gameOverElapsedTimeText.text = "Elapsed Time : " + ((int)(ScoreManager.Instance.ElapsedTime / 60)).ToString() + ":0" + sec.ToString();
            }

            if(m_gameOverShotNumberText)
                m_gameOverShotNumberText.text = "Shots : " + ScoreManager.Instance.ShotNumber;
        }
    }

    public void OnShotPowerChanged(float _power)
    {
        if(m_shotPowerGroup)
            m_shotPowerGroup.SetActive(true);
        if(m_shotPowerImage)
            m_shotPowerImage.transform.localScale = new Vector3(_power, 1, 1);
    }

    public void OnShot(float _power)
    {
        if(m_shotPowerGroup)
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
            GameManager.Instance.StartReplay();
    }

    public void OnNewGameButtonClicked()
    {
        if (GameManager.Instance)
            GameManager.Instance.RestartGame();

        InitGameUI();
    }

    public void OnSwitchGameStateEvent(GameManager.GameState _state)
    {
        switch (_state)
        {
            case GameManager.GameState.Shooting:
                break;
            case GameManager.GameState.ProcessingShot:
                if(m_replayButton)
                    m_replayButton.SetActive(false);
                break;
            case GameManager.GameState.EndOfShot:
                if(m_replayButton)
                    m_replayButton.SetActive(true);
                break;
            case GameManager.GameState.ProcessingReplay:
                break;
            default:
                break;
        }
    }
}