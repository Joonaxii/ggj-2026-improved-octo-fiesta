using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTextBack;
    [SerializeField] private TextMeshProUGUI _titleTextFront;
    [SerializeField] private TextMeshProUGUI _score;
    
    [SerializeField] private GameObject _highScoreText;
    
    public void Open(string gameResult, int score)
    {
        _titleTextBack.text = gameResult;
        _titleTextFront.text = gameResult;
        
        _score.text = score.ToString();

        var wasHigh = score >= PlayerPrefs.GetInt(PlayerPrefsValues.HIGHSCORE);
        _highScoreText.SetActive(wasHigh);

        PlayerPrefs.SetInt(PlayerPrefsValues.HIGHSCORE, score);
    }
}
