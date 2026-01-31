using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _indicator;
    
    // Update is called once per frame
    void Update()
    {
        SetValue((float)Math.Sin(Time.time));
    }
    
    public void SetValue(float value)
    {
        _fillImage.fillAmount = value;
    }
}
