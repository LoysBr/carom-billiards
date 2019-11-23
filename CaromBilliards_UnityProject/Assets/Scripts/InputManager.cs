using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void InputShot(float _power);
    public event InputShot InputShotEvent;
    public delegate void InputShotHolded(float _currentPower);
    public event InputShotHolded InputShotHoldEvent;
    public delegate void InputCameraRotation(float _angle);
    public event InputCameraRotation InputCameraRotationEvent;

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
        InputCameraRotationEvent?.Invoke(_angle);
    }
}