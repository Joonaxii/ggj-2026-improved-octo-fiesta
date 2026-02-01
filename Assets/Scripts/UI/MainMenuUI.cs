using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private ToggleButtonUI _sfxToggle;
    [SerializeField] private ToggleButtonUI _musicToggle;

    public void Open()
    {
        _score.text = PlayerPrefs.GetInt(PlayerPrefsValues.HIGHSCORE).ToString();
        _sfxToggle.CheckCross();
        _musicToggle.CheckCross();
    }
}
