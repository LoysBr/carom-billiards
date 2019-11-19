using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Tweak
    [Range(1, 10)]
    public float m_mouseXSpeedFactor;
    public float m_shotMaxPressDuration;
    #endregion

    #region Singleton
    private static InputManager m_instance;
    public static InputManager Instance
    {
        get
        {
            return m_instance;
        }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }
    #endregion


    #region Events
    public delegate void InputShot(float _power);
    public event InputShot InputShotEvent;
    public delegate void InputCameraRotation(float _angle);
    public event InputCameraRotation InputCameraRotationEvent;
    #endregion

    #region PrivateAttributes
    //Mouse
    private Vector3 m_previousMousePosition;
    private bool m_lastFrameMouseLeftClicked;

    //Space key
    private bool m_lastFrameSpaceKeyPressed;
    private float m_shotCurrentPressDuration = 0.0f;
    #endregion

    public void Start()
    {
        m_lastFrameSpaceKeyPressed = false;
    }

    public void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.WAITING_FOR_SHOT)
        {
            ManageMouseInputs();
            ManageSpacePressure();
        }
    }

    public void ManageSpacePressure()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if(m_lastFrameSpaceKeyPressed)
            {
                if(m_shotCurrentPressDuration < m_shotMaxPressDuration)
                {
                    m_shotCurrentPressDuration += Time.deltaTime;
                }
                else        // Max power !!!!
                {
                    m_shotCurrentPressDuration = m_shotMaxPressDuration;
                    InputShotEvent(GetShotPower());
                    m_shotCurrentPressDuration = 0.0f;
                }
            }

            m_lastFrameSpaceKeyPressed = true;
        }
        else
        {
            if(m_lastFrameSpaceKeyPressed)   // when we release the key
            {
                InputShotEvent(GetShotPower());
            }

            m_lastFrameSpaceKeyPressed = false;
            m_shotCurrentPressDuration = 0.0f;
        }
    }

    public float GetShotPower()
    {
        return m_shotCurrentPressDuration / m_shotMaxPressDuration;
    }

    public void ManageMouseInputs()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 newMousePos = Input.mousePosition;
            Vector3 deltaMouse;

            if (m_lastFrameMouseLeftClicked)
            {
                deltaMouse = newMousePos - m_previousMousePosition;
            }
            else
            {
                deltaMouse = Vector3.one;
            }

            m_previousMousePosition = newMousePos;

            InputCameraRotationEvent(- deltaMouse.x * m_mouseXSpeedFactor * Time.deltaTime * 0.04f);

            m_lastFrameMouseLeftClicked = true;
        }
        else
        {
            m_lastFrameMouseLeftClicked = false;
        }
    }
}
