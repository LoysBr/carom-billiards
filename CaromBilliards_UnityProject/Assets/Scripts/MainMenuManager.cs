using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public string       m_singlePlayerSceneName;
    public Slider       m_masterVolumeSlider;
    public GameObject   m_lastGameScoreParent;
    public Text         m_lastGameScoreText;
    public Text         m_lastGameTimeText;
    public Text         m_lastGameShotsText;

    void Start()
    {
        m_masterVolumeSlider.value = GameSettings.Instance.m_masterVolumeValue;

        if(ScoreManager.Instance)
        {
            if(ScoreManager.Instance.LastGameScore != null)
            {
                m_lastGameScoreParent.SetActive(true);
                m_lastGameScoreText.text = "Score : " + ScoreManager.Instance.LastGameScore.m_score;
                m_lastGameShotsText.text = "Shots : " + ScoreManager.Instance.LastGameScore.m_shotNumber;

                int sec = ((int)ScoreManager.Instance.LastGameScore.m_elapsedTime) % 60;
                if (sec >= 10)
                    m_lastGameTimeText.text = ((int)(ScoreManager.Instance.LastGameScore.m_elapsedTime / 60)).ToString() + ":" + sec.ToString();
                else
                    m_lastGameTimeText.text = ((int)(ScoreManager.Instance.LastGameScore.m_elapsedTime / 60)).ToString() + ":0" + sec.ToString();
            }
            else
            {
                m_lastGameScoreParent.SetActive(false);
            }
        }
    }

    public void OnNewGameButtonClick()
    {
        SceneManager.LoadScene(m_singlePlayerSceneName);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void OnMasterVolumeSliderChanged(float _newValue)
    {
        GameSettings.Instance.m_masterVolumeValue = _newValue;
    }
}
