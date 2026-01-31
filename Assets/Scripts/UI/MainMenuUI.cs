using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _score;

    public void Open()
    {
        _score.text = PlayerPrefs.GetInt(PlayerPrefsValues.HIGHSCORE).ToString();
    }
}
