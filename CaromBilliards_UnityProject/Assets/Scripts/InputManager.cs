using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void    InputShot(float _power);
    public event            InputShot InputShotEvent;
    public event            InputShot InputShotHoldEvent;
    public delegate void    InputMoveCamera(float _angle);
    public event            InputMoveCamera InputMoveCameraEvent;

    [HideInInspector]
    public float m_shotMaxHoldDuration;

    public void InvokeInputShotEvent(float _power)
    {
        InputShotEvent?.Invoke(_power);
    }
    public void InvokeInputShotHoldEvent(float _currentPower)
    {
        InputShotHoldEvent?.Invoke(_currentPower);
    }
    public void InvokeInputCameraRotationEvent(float _angle)
    {
        InputMoveCameraEvent?.Invoke(_angle);
    }
}