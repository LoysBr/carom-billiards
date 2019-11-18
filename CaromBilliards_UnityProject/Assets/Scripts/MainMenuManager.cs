using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public string m_singlePlayerSceneName;
    public Slider m_masterVolumeSlider;

    void Start()
    {
        m_masterVolumeSlider.value = GameSettings.Instance.m_masterVolumeValue;
    }


    void Update()
    {
        
    }

    public void OnNewGameButtonClick()
    {
        SceneManager.LoadScene(m_singlePlayerSceneName);
    }

    public void OnMasterVolumeSliderChanged(float _newValue)
    {
        GameSettings.Instance.m_masterVolumeValue = _newValue;
    }
}
