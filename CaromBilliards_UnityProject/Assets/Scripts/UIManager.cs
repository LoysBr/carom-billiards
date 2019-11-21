using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image        m_shotPowerImage;
    public GameObject   m_shotPowerGroup;

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
            GameManager.Instance.EndOfShotEvent += OnEndOfShot;        
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
