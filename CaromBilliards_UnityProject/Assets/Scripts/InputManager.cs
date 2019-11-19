using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{  
    [Range(1, 10)]
    public float m_mouseXSpeedFactor;

    private Vector3 m_previousMousePosition;
    private bool m_lastFrameMouseLeftClicked;

    public void Update()
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

            CameraManager.Instance.RotateCameraByAngle(deltaMouse.x * m_mouseXSpeedFactor * Time.deltaTime * 0.04f);

            m_lastFrameMouseLeftClicked = true;
        }
        else
        {
            m_lastFrameMouseLeftClicked = false;
        }
    }
}
