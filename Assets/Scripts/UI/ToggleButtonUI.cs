using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonUI : MonoBehaviour
{
    [SerializeField] private string _prefToggleName;
    [SerializeField] private Image _cross;

    public void Toggle()
    {
        var currentValue = PlayerPrefs.GetInt(_prefToggleName);

        PlayerPrefs.SetInt(_prefToggleName, currentValue == 1 ? 0 : 1);
        _cross.gameObject.SetActive(currentValue == 0);
    }
}
