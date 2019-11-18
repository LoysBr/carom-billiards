using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Persistent Singleton to share game settings across every scenes.
/// </summary>
public class GameSettings : MonoBehaviour
{
    private static GameSettings m_instance;
    public static GameSettings Instance { get
    {            
        return m_instance;
    } }


    [HideInInspector]
    public float m_masterVolumeValue;

    void Awake()
    {
        if(m_instance == null)     //first opening of menu
        {
            m_instance = this;
        }
        else                       //another scene is loaded
        {
            if(m_instance != this) //we only want to keep the original object
            {
                Destroy(this.gameObject);     
            }
        }

        DontDestroyOnLoad(this);

        m_masterVolumeValue = 1.0f;
    }          
}
