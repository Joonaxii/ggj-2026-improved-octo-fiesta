using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonUI : MonoBehaviour
{
    [SerializeField] private string _prefToggleName;
    [SerializeField] private Image _cross;

    public void Awake()
    {
        _cross.gameObject.SetActive(PlayerPrefs.GetInt(_prefToggleName) == 0);
    }

    public void Toggle()
    {
        var currentValue = PlayerPrefs.GetInt(_prefToggleName);
        var newValue = currentValue == 1 ? 0 : 1;
        
        PlayerPrefs.SetInt(_prefToggleName, newValue);
        _cross.gameObject.SetActive(newValue == 0);
    }
}
