using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public float        m_maxSpeed; //in m/s

    private Rigidbody   m_rigidBody;
    private bool        m_isMoving;
    
    #region Sound
    public AudioClip    m_cushionCollisionSound;
    public AudioClip    m_ballCollisionSound;
    public AudioSource  m_audioSource;
    #endregion        

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
    private List<BilliardObjects> m_lastShotCollisions;

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

        m_isMoving = true;

        PlayBallCollisionSound(m_rigidBody.velocity.magnitude / m_maxSpeed);
    }

    //collision with cushions
    public void OnTriggerEnter(Collider other)
    {
        Vector3 newVelocity = m_rigidBody.velocity - 2 * Vector3.Dot(m_rigidBody.velocity, other.transform.forward) * other.transform.forward;
        m_rigidBody.velocity = newVelocity;

        m_lastShotCollisions.Add(BilliardObjects.Cushion);

        PlayCushionCollisionSound(m_rigidBody.velocity.magnitude / m_maxSpeed);
    }

    void Update()
    {
        if(m_isMoving)
        {
            HelpBallStop();
        }
    }

    public void OnPlayerShot(float _power, Vector3 _dir)
    {       
        m_rigidBody.velocity = m_maxSpeed * _power * _dir;
        m_isMoving = true;

        PlayBallCollisionSound(_power);
    }

    public bool IsStopped()
    {
        return !m_isMoving;
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

    /// <summary>
    /// While using physic, after a shot, balls take a lot
    /// of time to decrease their velocity down to pure Zero
    /// We will just fake a bit and force their velocity to 0 when it's near.
    /// </summary>
    public void HelpBallStop()
    {
        if (m_rigidBody.velocity.magnitude <= 0.005)
        {
            m_rigidBody.velocity = Vector3.zero;
            m_isMoving = false;
        }
    }

    public void PlayCushionCollisionSound(float _volume)
    {
        if(GameSettings.Instance)
            m_audioSource.PlayOneShot(m_cushionCollisionSound, _volume * GameSettings.Instance.m_masterVolumeValue);
        else
            m_audioSource.PlayOneShot(m_cushionCollisionSound, _volume);
    }

    public void PlayBallCollisionSound(float _volume)
    {
        if (GameSettings.Instance)
            m_audioSource.PlayOneShot(m_ballCollisionSound, _volume * GameSettings.Instance.m_masterVolumeValue);
        else
            m_audioSource.PlayOneShot(m_ballCollisionSound, _volume);
    }
}
