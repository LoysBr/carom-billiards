using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image        m_shotPowerImage;
    public GameObject   m_shotPowerGroup;

    void Start()
    {
        m_shotPowerGroup.SetActive(false);

        InputManager.Instance.InputShotEvent += OnShot;
        InputManager.Instance.InputShotHoldEvent += OnShotPowerChanged;;
    }

    
    void Update()
    {
        
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

}
