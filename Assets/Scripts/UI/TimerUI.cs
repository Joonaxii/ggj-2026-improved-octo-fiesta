using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private RectTransform _wheel;
    [SerializeField] private RectTransform _sunIndicator;
    [SerializeField] private RectTransform _moonIndicator;
    
    public void ResetVisuals(float initialValue = 0)
    {
        UpdateTimeWheelRotation(initialValue);
    }
    
    // 0 -> 1 does one full rotation of the wheel, goes clock wise
    // Initial position for 3 o'clock is roughly 0.15
    public void UpdateTimeWheelRotation(float value)
    {
        //_wheel.rotation = Quaternion.Euler(0, 0, -(value * 360 + 270));
        _wheel.rotation = Quaternion.Euler(0, 0, -Mathf.LerpUnclamped(90.0f, 360, value));
        
        _sunIndicator.rotation = Quaternion.Euler(0, 0, -_wheel.rotation.eulerAngles.y);
        _moonIndicator.rotation = Quaternion.Euler(0, 0, -_wheel.rotation.eulerAngles.y);
    }
}
