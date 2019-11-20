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

    public void OnCollisionEnter(Collision collision)
    {
        //Vector3 colliderDir = Vector3.Cross(collision.GetContact(0).normal, Vector3.up).normalized;
        //Debug.Log("colliderDir : " + colliderDir);

        //Vector3 colliderNorm = collision.GetContact(0).normal;
        //Debug.Log("colliderNorm  : " + colliderNorm);
       

        //float collisionAngle = Vector3.Angle(colliderDir, m_direction);
        //Debug.Log("angle : " + collisionAngle);

        //Vector3 newDir = Mathf.Cos(collisionAngle) * colliderDir + Mathf.Sin(collisionAngle) * colliderNorm;

        //Debug.Log("previousDir : " + m_direction);
        //Debug.Log("newDir : " + newDir);

        //m_direction = newDir;
    }

    //collision with cushions
    public void OnTriggerEnter(Collider other)
    {
        Vector3 newDir = m_direction - 2 * Vector3.Dot(m_direction, other.transform.forward) * other.transform.forward;
        m_direction = newDir.normalized;
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
