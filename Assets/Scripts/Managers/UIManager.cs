using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private GameResultUI _gameResultUI;
    [SerializeField] private HUD _hud;

    protected override void Awake()
    {
        base.Awake();
    }
    
    public void OpenMainMenuUI()
    {
        _mainMenuUI.gameObject.SetActive(true);
        _mainMenuUI.Open();
    }
    
    public void OpenHUD()
    {
        _hud.gameObject.SetActive(true);
        _hud.Open();
    }

    public void OpenGameResultUI(string gameResult, int score)
    {
        _gameResultUI.gameObject.SetActive(true);
        _gameResultUI.Open(gameResult, score);
    }
}
