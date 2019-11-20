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

    private BilliardObjects m_selfTag;

    //to store every collisions from last shot
    //for basic rules it's a bit too much but may be 
    //usefull if we add new rules
    public List<BilliardObjects> m_lastShotCollisions;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        ResetLastShotCollisions();

        switch (gameObject.tag)
        {
            case "WhiteBall":
                m_selfTag = BilliardObjects.WhiteBall;
                break;
            case "YellowBall":
                m_selfTag = BilliardObjects.YellowBall;
                break;
            case "RedBall":
                m_selfTag = BilliardObjects.RedBall;
                break;
            default:
                break;
        }
    }   

    public void ResetLastShotCollisions()
    {
        m_lastShotCollisions = new List<BilliardObjects>(20);
    }

    //collision with other balls
    public void OnCollisionEnter(Collision collision)
    {
        switch(collision.collider.gameObject.tag)
        {
            case "WhiteBall":
                m_lastShotCollisions.Add(BilliardObjects.WhiteBall);
                break;
            case "YellowBall":
                m_lastShotCollisions.Add(BilliardObjects.YellowBall);
                break;
            case "RedBall":
                m_lastShotCollisions.Add(BilliardObjects.RedBall);
                break;
            default:
                break;
        }
    }

    //collision with cushions
    public void OnTriggerEnter(Collider other)
    {
        Vector3 newVelocity = m_rigidBody.velocity - 2 * Vector3.Dot(m_rigidBody.velocity, other.transform.forward) * other.transform.forward;
        m_rigidBody.velocity = newVelocity;

        m_lastShotCollisions.Add(BilliardObjects.Cushion);
    }

    void Update()
    {
        
    }

    public void OnBallShot(float _power, Vector3 _dir)
    {               
        m_rigidBody.velocity = m_maxSpeed * _power * _dir;
    }

    public bool IsStopped()
    {
        return m_rigidBody.velocity == Vector3.zero;
    }

    public bool HasCollidedWithTwoOtherBalls()
    {
        HashSet<BilliardObjects> collisions = new HashSet<BilliardObjects>();
        foreach(BilliardObjects obj in m_lastShotCollisions)
        {
            if(obj != m_selfTag && obj != BilliardObjects.Cushion)
            {
                collisions.Add(obj);
                if (collisions.Count == 2)
                    return true;
            }
        }

        return false;
    }
}
