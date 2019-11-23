using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CaromBilliards;

/// <summary>
/// Needs ref to a GameManager
/// </summary>
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

    private void Start()
    {
        InitGameUI();
              
        if (GameManager.Instance)
        {
            GameManager.Instance.SwitchStateEvent += OnSwitchGameStateEvent;
            GameManager.Instance.GameOverEvent += OnGameOver;
            GameManager.Instance.PlayerShotEvent += OnShot;
            GameManager.Instance.InputShotHoldEvent += OnShotPowerChanged;
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
                m_scorePanel.SetActive(true);
                if (m_scoreText)
                    m_scoreText.text = Utils.GetScoreString(ScoreManager.Instance.CurrentGameScore);

                if (m_elapsedTimeText)
                    m_elapsedTimeText.text = Utils.GetElapsedTimeString(ScoreManager.Instance.ElapsedTime, true);

                if (m_shotNumberText)
                    m_shotNumberText.text = Utils.GetShotsNumberString(ScoreManager.Instance.ShotNumber);
            }
            else
            {
                m_scorePanel.SetActive(false);
            }
        }
    }

    private void OnGameOver()
    {
        if (m_gameOverPanel)
        { 
            if (ScoreManager.Instance)
            {
                m_gameOverPanel.SetActive(true);

                if (m_gameOverScoreText)
                    m_gameOverScoreText.text = Utils.GetScoreString(ScoreManager.Instance.CurrentGameScore);

                if (m_gameOverElapsedTimeText)
                    m_gameOverElapsedTimeText.text = Utils.GetElapsedTimeString(ScoreManager.Instance.ElapsedTime);

                if (m_gameOverShotNumberText)
                    m_gameOverShotNumberText.text = Utils.GetShotsNumberString(ScoreManager.Instance.ShotNumber);
            }
            else
                m_gameOverPanel.SetActive(false);
        }
    }

    private void OnShotPowerChanged(float _power)
    {
        if (GameManager.Instance && GameManager.Instance.CurrentGameState == GameManager.GameState.Shooting)
        {
            if (m_shotPowerGroup)
                m_shotPowerGroup.SetActive(true);
            if (m_shotPowerImage)
                m_shotPowerImage.transform.localScale = new Vector3(_power, 1, 1);
        }
    }

    private void OnShot(float _power)
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

    private void OnSwitchGameStateEvent(GameManager.GameState _state)
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