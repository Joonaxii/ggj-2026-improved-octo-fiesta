using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private RectTransform _wheel;
    [SerializeField] private RectTransform _sunIndicator;
    [SerializeField] private RectTransform _moonIndicator;
    
    // Update is called once per frame
    void Update()
    {
        _wheel.rotation = Quaternion.Euler(0, 0, -(Time.time * 360) *0.5f );
        
        _sunIndicator.rotation = Quaternion.Euler(0, 0, -_wheel.rotation.eulerAngles.y);
        _moonIndicator.rotation = Quaternion.Euler(0, 0, -_wheel.rotation.eulerAngles.y);
    }
}
