using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _indicator;
    
    [SerializeField] private Sprite[] _indicatorSprites =  new Sprite[4];

    public void ResetVisuals()
    {
        UpdateIndicatorState(0);
        SetMeterValue(0);
    }
    
    // Sets the indicator sprite, change this with the suspicion level of nearby party goers
    public void UpdateIndicatorState(int suspicionLevel)
    {
        var spriteIndex = Mathf.Clamp(suspicionLevel, 0, _indicatorSprites.Length - 1);
        _indicator.sprite = _indicatorSprites[spriteIndex];
    }
    
    public void SetMeterValue(float value)
    {
        // 0 -> 1
        _fillImage.fillAmount = value;
    }
}
