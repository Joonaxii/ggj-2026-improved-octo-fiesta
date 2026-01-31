using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameplayMusic;
    [SerializeField] private float _crossfadeSmooth;

    [SerializeField] private AudioClip _victoryMusic;
    [SerializeField] private AudioClip _loseMusic;

    private AudioSource _jinglePlayer;
    private AudioSource _menuMusicPlayer;
    private AudioSource _gameplayMusicPlayer;
    
    private float _crossFade;
    private float _velocity;
    private bool _isGameplay;
    private bool _playingJingle;
    
    private List<AudioSource> _audioSources;
    private const int INITIAL_SOURCES = 8;

    protected override void Awake()
    {
        base.Awake();
        
        _audioSources = new List<AudioSource>();

        for (int i = 0; i < INITIAL_SOURCES; i++)
        {
            AddAudioSource();
        }
        
        _menuMusicPlayer = AddMusicSource("Menu Music",true, _menuMusic, 1);
        _gameplayMusicPlayer = AddMusicSource("Gameplay Music", true, _gameplayMusic, 0);
        
        _gameplayMusicPlayer.Play();
        _menuMusicPlayer.Play();
        
        _jinglePlayer = AddMusicSource("Jingle Player", false, null, 1);
    }

    public void ToggleMusic()
    {
        var toggle = PlayerPrefs.GetInt(PlayerPrefsValues.MUSIC_TOGGLE);
        
        if (toggle == 1)
        {
            _menuMusicPlayer.time = 0;
            _gameplayMusicPlayer.time = 0;
            
            _menuMusicPlayer.Play();    
            _gameplayMusicPlayer.Play();    
        }
        else
        {
            _menuMusicPlayer.Stop();
            _gameplayMusicPlayer.Stop();
        }
    }
    
    public void PlayVictoryMusic()
    {
        if (PlayerPrefs.GetInt(PlayerPrefsValues.MUSIC_TOGGLE) == 0)
        {
            return;
        }
        PlayJingle(_victoryMusic);
    }

    public void PlayLoseMusic()
    {
        if (PlayerPrefs.GetInt(PlayerPrefsValues.MUSIC_TOGGLE) == 0)
        {
            return;
        }
        PlayJingle(_loseMusic);
    }

    private void PlayJingle(AudioClip clip)
    {
        _jinglePlayer.clip = clip;
        _jinglePlayer.time = 0;
        _jinglePlayer.Play();
        
        _playingJingle = true;
        
        _gameplayMusicPlayer.volume = 0;
        _menuMusicPlayer.volume = 0;
    }
    
    public void ToggleGameplay(bool isGameplay)
    {
        _isGameplay = isGameplay;
    }
    
    private void Update()
    {
        if (_playingJingle)
        {
            if (!_jinglePlayer.isPlaying)
            {
                _playingJingle = false;
                _jinglePlayer.clip = null;
            }
            return;
        }
        
        
        _crossFade = Mathf.SmoothDamp(_crossFade, _isGameplay ? 1 : 0, ref _velocity, _crossfadeSmooth);
        
        _gameplayMusicPlayer.volume = _crossFade;
        _menuMusicPlayer.volume = 1.0f - _crossFade;
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayVictoryMusic();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayLoseMusic();
        }
    }

    private AudioSource AddMusicSource(string name, bool loop, AudioClip clip, float volume)
    {
        var newSource = new GameObject(name).AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.transform.SetParent(transform);
        newSource.loop = loop;
        newSource.clip = clip;
        newSource.volume = volume;
        return newSource;
    }
    
    private AudioSource AddAudioSource()
    {
        var source = new GameObject("Audio Source").AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.transform.SetParent(transform);
        _audioSources.Add(source);
        return source;
    }

    private void PlayClip(AudioSource source, AudioClip clip, float pitch)
    {
        source.clip = clip;
        source.pitch = pitch;
            
        source.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip, float pitch)
    {
        if (PlayerPrefs.GetInt(PlayerPrefsValues.SFX_TOGGLE) == 0)
        {
            return;
        }
        
        foreach (var audioSource in _audioSources)
        {
            if (audioSource.isPlaying) continue;
            PlayClip(audioSource, clip, pitch);

            return;
        }

        var newSource = AddAudioSource();
        PlayClip(newSource, clip, pitch);
    }
}
