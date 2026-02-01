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
    private bool _playingJingle;

    private bool _musicOK;
    private bool _audioOK;
    
    private List<AudioSource> _audioSources;
    private const int INITIAL_SOURCES = 8;

    protected override void Awake()
    {
        base.Awake();

        _audioOK = PlayerPrefs.GetInt(PlayerPrefsValues.SFX_TOGGLE, 1) != 0;
        _musicOK = PlayerPrefs.GetInt(PlayerPrefsValues.MUSIC_TOGGLE, 1) != 0;

        _audioSources = new List<AudioSource>();

        for (int i = 0; i < INITIAL_SOURCES; i++)
        {
            AddAudioSource();
        }
        
        _menuMusicPlayer = AddMusicSource("Menu Music",true, _menuMusic, 1);
        _gameplayMusicPlayer = AddMusicSource("Gameplay Music", true, _gameplayMusic, 0);

        if (_musicOK)
        {
            _gameplayMusicPlayer.Play();
            _menuMusicPlayer.Play();
        }
        
        _jinglePlayer = AddMusicSource("Jingle Player", false, null, 1);
    }

    public void ToggleMusic()
    {
        _musicOK = PlayerPrefs.GetInt(PlayerPrefsValues.MUSIC_TOGGLE, 1) != 0;
        if (_musicOK)
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
        if (!_musicOK)
        {
            return;
        }
        PlayJingle(_victoryMusic);
    }

    public void PlayLoseMusic()
    {
        if (!_musicOK)
        {
            return;
        }
        PlayJingle(_loseMusic);
    }

    private void PlayJingle(AudioClip clip)
    {
        _jinglePlayer.clip = clip;
        _jinglePlayer.time = 0;
        _jinglePlayer.loop = false;
        _jinglePlayer.volume = 0.65f;
        _jinglePlayer.Play();
        
        _playingJingle = true;
        
        _gameplayMusicPlayer.volume = 0;
        _menuMusicPlayer.volume = 0;
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
        var state = GameManager.Instance.State;

        bool isInGameplay = state == GameManager.GameState.InGame;
        _crossFade = Mathf.SmoothDamp(_crossFade, isInGameplay ? 1 : 0, ref _velocity, _crossfadeSmooth);
        
        _gameplayMusicPlayer.volume = (_crossFade) * 0.65f;
        _menuMusicPlayer.volume = (1.0f - _crossFade) * 0.65f;
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

    private void PlayClip(AudioSource source, AudioClip clip, float pitch, Vector3 position)
    {
        source.clip = clip;
        source.pitch = pitch;
        source.transform.position = position;
        source.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip, float pitch)
    {
        _audioOK = PlayerPrefs.GetInt(PlayerPrefsValues.SFX_TOGGLE, 1) != 0;

        if (!_audioOK)
        {
            return;
        }
        
        foreach (var audioSource in _audioSources)
        {
            if (audioSource.isPlaying) continue;
            audioSource.spatialize = false;
            audioSource.spatialBlend = 0;
            PlayClip(audioSource, clip, pitch, Vector3.zero);

            return;
        }

        var newSource = AddAudioSource();
        PlayClip(newSource, clip, pitch, Vector3.zero);
    }

    public void PlaySoundAtLocation(AudioClip clip, float pitch, Vector3 position)
    {
        _audioOK = PlayerPrefs.GetInt(PlayerPrefsValues.SFX_TOGGLE, 1) != 0;

        if (!_audioOK)
        {
            return;
        }
        
        position.z = -10;
        
        foreach (var audioSource in _audioSources)
        {
            if (audioSource.isPlaying) continue;
            audioSource.spatialize = true;
            audioSource.spatialBlend = 1;
            PlayClip(audioSource, clip, pitch, position);

            return;
        }

        var newSource = AddAudioSource();
        newSource.spatialize = true;
        newSource.spatialBlend = 1;
        PlayClip(newSource, clip, pitch, position);
    }
}
