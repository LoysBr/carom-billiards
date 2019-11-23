using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private PlayerPreferences m_playerPref;

    public string       m_singlePlayerSceneName;
    public Slider       m_masterVolumeSlider;
    public Dropdown     m_difficultyDropdown;
    public GameObject   m_lastGameScoreParent;
    public Text         m_lastGameScoreText;
    public Text         m_lastGameTimeText;
    public Text         m_lastGameShotsText;

    void Start()
    {
        m_playerPref = FindObjectOfType<PlayerPreferences>();

        if (m_playerPref)
        {
            m_masterVolumeSlider.gameObject.SetActive(true);
            m_masterVolumeSlider.value = m_playerPref.MasterVolume;

            m_difficultyDropdown.gameObject.SetActive(true);
            m_difficultyDropdown.AddOptions(new List<string>(m_playerPref.GetDifficultyValuesStrings()));
            m_difficultyDropdown.value = (int) m_playerPref.Difficulty;
        }
        else
        {
            m_masterVolumeSlider.gameObject.SetActive(false);
            m_difficultyDropdown.gameObject.SetActive(false);
        }      


        if (ScoreManager.Instance)
        {
            if(ScoreManager.Instance.LastGameScore != null)
            {
                m_lastGameScoreParent.SetActive(true);
                m_lastGameScoreText.text = "Score : " + ScoreManager.Instance.LastGameScore.m_score;
                m_lastGameShotsText.text = "Shots : " + ScoreManager.Instance.LastGameScore.m_shotNumber;

                int sec = ((int)ScoreManager.Instance.LastGameScore.m_elapsedTime) % 60;
                if (sec >= 10)
                    m_lastGameTimeText.text = "Elapsed Time : " + ((int)(ScoreManager.Instance.LastGameScore.m_elapsedTime / 60)).ToString() + ":" + sec.ToString();
                else
                    m_lastGameTimeText.text = "Elapsed Time : " + ((int)(ScoreManager.Instance.LastGameScore.m_elapsedTime / 60)).ToString() + ":0" + sec.ToString();
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
        if(m_playerPref)
            m_playerPref.MasterVolume = _newValue;        
    }

    public void OnDifficultyChanged(int _difficulty)
    {
        if(m_playerPref)
            m_playerPref.Difficulty = (PlayerPreferences.GameDifficulty)_difficulty;
    }
}
