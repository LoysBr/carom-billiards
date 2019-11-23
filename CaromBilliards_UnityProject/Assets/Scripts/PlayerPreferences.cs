using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Private Singleton other objects optionnaly find via FindObjectOfType
/// Not necessary to play the game
/// </summary>
public class PlayerPreferences : MonoBehaviour
{
    private static PlayerPreferences m_instance;

    [HideInInspector]
    public float m_masterVolumeValue;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public Difficulty m_difficulty;
    
    void Awake()
    {
        if(m_instance == null)     //first opening of menu
        {
            m_instance = this;
            DontDestroyOnLoad(this);
        }
        else                       //another scene is loaded
        {
            if(m_instance != this) //we only want to keep the original object
            {
                DestroyImmediate(this.gameObject);     
            }
        }        

        m_masterVolumeValue = 1.0f;
    }          
}
