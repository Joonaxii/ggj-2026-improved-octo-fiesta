using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private GameResultUI _gameResultUI;
    [SerializeField] private HUD _hud;
    [SerializeField] private CanvasGroup _screenFade;
    [SerializeField] private float _fadeInTime;

    private IEnumerator _starter;
    private int _counter = 0;

    protected override void Awake()
    {
        base.Awake();
        _screenFade.alpha = 0;
    }

    private void Start()
    {
        CloseHUD();
        CloseGameResultUI();
        OpenMainMenuUI();
    }

    public void StartGame()
    {
        if(_starter != null) { return; }
        _starter = GameStarter();
        this.StartCoroutine(_starter);
    }
    
    public void Restart()
    {
        if(_starter != null) { return; }
        _starter = GameReStarter();
        this.StartCoroutine(_starter);
    }
    
    public void ToMenu()
    {
        if(_starter != null) { return; }
        _starter = MenuSwitcher();
        this.StartCoroutine(_starter);
    }

    private IEnumerator MenuSwitcher()
    {
        var mngr = GameManager.Instance;

        float t = 0;
        while(t <= _fadeInTime)
        {
            t += Time.deltaTime;

            _screenFade.alpha = t / Mathf.Max(t, 1.0f);
            yield return null;
        }
        _screenFade.alpha = 1.0f;
        mngr.ResetGame();
        CloseHUD();
        CloseGameResultUI();
        OpenMainMenuUI();
        
        t = 0;
        while (t <= _fadeInTime)
        {
            t += Time.deltaTime;

            _screenFade.alpha = 1.0f - t / Mathf.Max(t, 1.0f);
            yield return null;
        }
        _screenFade.alpha = 0.0f;
        _starter = null;
    }

    private IEnumerator GameStarter()
    {
        var mngr = GameManager.Instance;

        float t = 0;
        while(t <= _fadeInTime)
        {
            t += Time.deltaTime;

            _screenFade.alpha = t / Mathf.Max(t, 1.0f);
            yield return null;
        }
        _screenFade.alpha = 1.0f;
        mngr.ResetGame();
        OpenHUD();
        CloseMainMenuUI();
        UpdateGoers();

        t = 0;
        while (t <= _fadeInTime)
        {
            t += Time.deltaTime;

            _screenFade.alpha = 1.0f - t / Mathf.Max(t, 1.0f);
            yield return null;
        }
        _screenFade.alpha = 0.0f;

        // TODO: Play Start animation thing

        mngr.StartGame();

        _starter = null;
    }

    private IEnumerator GameReStarter()
    {
        var mngr = GameManager.Instance;

        float t = 0;
        while(t <= _fadeInTime)
        {
            t += Time.deltaTime;

            _screenFade.alpha = t / Mathf.Max(t, 1.0f);
            yield return null;
        }
        _screenFade.alpha = 1.0f;
        mngr.ResetGame();
        OpenHUD();
        CloseGameResultUI();
        CloseMainMenuUI();
        UpdateGoers();

        t = 0;
        while (t <= _fadeInTime)
        {
            t += Time.deltaTime;

            _screenFade.alpha = 1.0f - t / Mathf.Max(t, 1.0f);
            yield return null;
        }
        _screenFade.alpha = 0.0f;

        // TODO: Play Start animation thing

        mngr.StartGame();

        _starter = null;
    }

    public void UpdateTime(float time)
    {
        _hud.UpdateTimer(time);
    }

    public void UpdateGoers()
    {
        _hud.UpdateBloodDrop(PartyManager.Instance.GoersLeft, PartyManager.Instance.TotalGoers);
    }
    
    public void ClickRGB()
    {
        _counter++;
        if(_counter >= 10)
        {
            GameManager.Instance.RainbowMode = true;
            _counter = 0;
        }
    }

    public void OpenMainMenuUI()
    {
        _counter = 0;
        GameManager.Instance.RainbowMode = false;
        GameManager.Instance.State = GameManager.GameState.Menu;
        _mainMenuUI.gameObject.SetActive(true);
        _mainMenuUI.Open();
    }
    
    
    public void CloseMainMenuUI()
    {
        _mainMenuUI.gameObject.SetActive(false);
    }
    
    public void CloseHUD()
    {
        _hud.gameObject.SetActive(false);
    }
    
    
    public void CloseGameResultUI()
    {
        _gameResultUI.gameObject.SetActive(false);
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
