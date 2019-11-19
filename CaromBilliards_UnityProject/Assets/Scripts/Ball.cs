using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public float        m_maxSpeed; //in m/s
    [HideInInspector]
    public float        m_friction; //also in m/s

    private Vector3     m_direction; 

    private float       m_currentSpeed;

    void Start()
    {
        m_currentSpeed = 0;
    }

    void Update()
    {
        //movement
        if(m_currentSpeed > 0)
        {
            transform.position += m_direction * m_currentSpeed * Time.deltaTime;

            m_currentSpeed -= m_friction * Time.deltaTime;
        }
    }

    public void OnBallShot(float _power, Vector3 _dir)
    {
        m_currentSpeed = m_maxSpeed * _power;
        m_direction = _dir;
    }
}
