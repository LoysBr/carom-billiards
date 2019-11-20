using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public float        m_maxSpeed; //in m/s

    private Rigidbody   m_rigidBody;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }   

    //collision with other balls
    public void OnCollisionEnter(Collision collision)
    {
        
    }

    //collision with cushions
    public void OnTriggerEnter(Collider other)
    {
        Vector3 newVelocity = m_rigidBody.velocity - 2 * Vector3.Dot(m_rigidBody.velocity, other.transform.forward) * other.transform.forward;
        m_rigidBody.velocity = newVelocity;
    }

    void Update()
    {
        
    }

    public void OnBallShot(float _power, Vector3 _dir)
    {               
        m_rigidBody.velocity = m_maxSpeed * _power * _dir;
    }
}
