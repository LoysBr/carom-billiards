using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public float        m_maxSpeed; //in m/s

    private Rigidbody   m_rigidBody;

    public enum BilliardObjects
    {
        Cushion,
        WhiteBall,
        YellowBall,
        RedBall
    }

    //to store every collisions from last shot
    //for basic rules it's a bit too much but may be 
    //usefull if we add new rules
    public List<BilliardObjects> m_lastShotCollisions;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        ResetLastShotCollisions();
    }   

    public void ResetLastShotCollisions()
    {
        m_lastShotCollisions = new List<BilliardObjects>(20);
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
