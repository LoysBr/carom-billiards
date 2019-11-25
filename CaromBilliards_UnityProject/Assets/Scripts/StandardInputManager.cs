using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton. Only designed to be in a game scene
/// Has no reference to any other Object
/// Other objects are listening to its events
/// </summary>
public class StandardInputManager : InputManager
{
    #region Tweak
    [Range(1, 6)]
    [SerializeField]
    private float m_mouseXSpeedFactor;
    //[Range(1, 6)]
    [SerializeField]
    private float m_mouseWheelFactor;
    #endregion


    //Mouse
    private Vector3     m_previousMousePosition;
    private bool        m_lastFrameMouseLeftClicked;

    //Space key
    private bool        m_lastFrameSpaceKeyPressed;
    private float       m_shotCurrentPressDuration = 0.0f;    

    private void Start()
    {
        m_lastFrameSpaceKeyPressed = false;
    }

    private void Update()
    {
        ManageMouseInputs();
        ManageSpacePressure();         
    }

    private void ManageSpacePressure()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if(m_lastFrameSpaceKeyPressed)
            {
                if(m_shotCurrentPressDuration < m_shotMaxHoldDuration)
                {
                    m_shotCurrentPressDuration += Time.deltaTime;
                    InvokeInputShotHoldEvent(GetShotPower());
                }
                else        // Max power !!!!
                {
                    m_shotCurrentPressDuration = m_shotMaxHoldDuration;

                    InvokeInputShotHoldEvent(1);
                    InvokeInputShotEvent(1);

                    m_shotCurrentPressDuration = 0.0f;
                    m_lastFrameSpaceKeyPressed = false;
                }
            }      
            else
                m_lastFrameSpaceKeyPressed = true;
        }
        else
        {
            if(m_lastFrameSpaceKeyPressed)   // when we release the key
            {
                InvokeInputShotEvent(GetShotPower());
            }

            m_lastFrameSpaceKeyPressed = false;
            m_shotCurrentPressDuration = 0.0f;
        }
    }

    private float GetShotPower()
    {
        return m_shotCurrentPressDuration / m_shotMaxHoldDuration;
    }

    private void ManageMouseInputs()
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

            InvokeInputCameraRotationEvent(- deltaMouse.x * m_mouseXSpeedFactor * Time.deltaTime * 0.04f);

            m_lastFrameMouseLeftClicked = true;
        }
        else
        {
            m_lastFrameMouseLeftClicked = false;
        }

        Debug.Log(Input.mouseScrollDelta.y);
        if(Input.mouseScrollDelta.y != 0)
        {
            float delta = Input.mouseScrollDelta.y * m_mouseWheelFactor;
            InvokeInputChangeCameraHeightEvent(-delta);
        }
    }       
}
