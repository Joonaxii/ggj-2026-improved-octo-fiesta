using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static DebugView DebugFlags
    {
        get => Instance != null ? Instance._debugView : DebugView.All;
    }

    public float CurrentTime => _time;

    public bool RainbowMode
    {
        get => _rainbowMode;
        set => _rainbowMode = value;
    }

    [SerializeField] private bool _rainbowMode;
    [SerializeField] private DebugView _debugView = DebugView.All;
    [SerializeField, Min(0.0f)] private float _rainbowRate = 1.0f;
    [SerializeField] private float _nightLength = 60;
    [SerializeField] private Transform _playerSpawn;

    public GameState State
    {
        get => _state;
        set => _state = value;
    }

    public Color RainbowColor => _rainbowColor;

    private Gradient _rainbowModeGradient;
    private Color _rainbowColor;
    private GameState _state = GameState.Menu;
    private float _time;
    private int _score;
    private Player _player;

    protected override void Awake()
    {
        base.Awake();
        _player = GameObject.FindObjectOfType<Player>();
        _player.Init();

        _rainbowModeGradient = new Gradient();
        GradientColorKey[] keys = new GradientColorKey[8];
        for (int i = 0; i < 8; i++)
        {
            float t = i / 7.0f;
            keys[i] = new GradientColorKey(Color.HSVToRGB(i / 7.0f, 1.0f, 1.0f), t);
        }
        _rainbowModeGradient.colorKeys = keys;
        _rainbowModeGradient.mode = GradientMode.Blend;

    }
    private void Start()
    {
        ResetGame();
    }

    public void ResetGame()
    {
        _time = 0;
        _score = 0;
        _state = GameState.EnteringGame;
        PartyManager.Instance.GetOutOfMyHouse();
        PartyManager.Instance.SpawnParty();
        _player.Movement.ResetMove();
        _player.transform.position = _playerSpawn.position;
    }
    
    public void StartGame()
    {
        _time = 0;
        _state = GameState.InGame;
    }

    private float _timer;
    private string _resultReason;

    public void Win(string reason)
    {
        _timer = 0;
        _resultReason = reason;
        _state = GameState.GoingToWin;
        AudioManager.Instance.PlayVictoryMusic();
    }

    public void Lose(string reason)
    {
        _timer = 0;
        _resultReason = reason;
        _state = GameState.GoingToLose;
        AudioManager.Instance.PlayLoseMusic();
    }

    public void AddScore(int amount)
    {
        _score += amount;
        _score = Math.Max(_score, 0);
    }

    void Update()
    {
        _rainbowColor = _rainbowModeGradient.Evaluate(Mathf.Repeat(_time * _rainbowRate, 1.0f));

        float nightProgress = _time / Mathf.Max(_nightLength, 1.0f);

        LocatorSystem.Instance.Tick();
        LightingManager.Instance.Tick(nightProgress);

        switch (_state)
        {
            case GameState.AtWinScreen:
                _player.TickMovement();
                break;

            case GameState.GoingToWin:
                _timer += Time.deltaTime;
                if (_timer < 1.0f) { break; }

                UIManager.Instance.CloseHUD();
                UIManager.Instance.OpenGameResultUI(_resultReason, _score);
                _state = GameState.AtWinScreen;

                _player.TickMovement();

                _timer = 0;
                break;

            case GameState.GoingToLose:
                _timer += Time.deltaTime;
                if(_timer < 1.0f) { break; }

                UIManager.Instance.CloseHUD();
                UIManager.Instance.OpenGameResultUI(_resultReason, _score);
                _state = GameState.AtLoseScreen;

                _timer = 0;
                break;

            case GameState.InGame:
                UIManager.Instance.UpdateTime(nightProgress);

                if(_time >= _nightLength)
                {
                    Lose("Time Out!");
                    _time = _nightLength;

                    UIManager.Instance.UpdateTime(1.0f);
                    _timer = 0;
                    break;
                }

                _player.TickMovement();
                PartyManager.Instance.TickMovement();

                _player.TickInteraction();
                PartyManager.Instance.TickBehaviour();
                _time += Time.deltaTime;
                break;
            case GameState.Menu:
                PartyManager.Instance.TickMovement();
                PartyManager.Instance.TickBehaviour();
                _time += Time.deltaTime;
                break;
        }
    }

    [System.Flags]
    public enum DebugView
    {
        None = 0x00,
        LocatorGrid = 0x01,
        Nodes = 0x02,
        PathFinding = 0x04,


        All = -1,
    }

    public enum GameState
    {
        Menu,
        EnteringGame,
        InGame,
        
        GoingToWin,
        AtWinScreen,

        GoingToLose,
        AtLoseScreen,
    }
}
