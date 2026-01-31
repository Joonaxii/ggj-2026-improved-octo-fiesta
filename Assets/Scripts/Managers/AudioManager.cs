using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameplayMusic;
    [SerializeField] private float _crossfadeSmooth;

    private AudioSource _menuMusicPlayer;
    private AudioSource _gameplayMusicPlayer;
    
    private float _crossFade;
    private float _velocity;
    private bool _isGameplay;
    
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
        
        _menuMusicPlayer = new GameObject("Menu Music Source").AddComponent<AudioSource>();
        _menuMusicPlayer.playOnAwake = false;
        _menuMusicPlayer.transform.SetParent(transform);
        _menuMusicPlayer.loop = true;
        _menuMusicPlayer.clip = _menuMusic;
        
        _gameplayMusicPlayer = new GameObject("Gameplay Music Source").AddComponent<AudioSource>();
        _gameplayMusicPlayer.playOnAwake = false;
        _gameplayMusicPlayer.transform.SetParent(transform);
        _gameplayMusicPlayer.loop = true;
        _gameplayMusicPlayer.clip = _gameplayMusic;
        _gameplayMusicPlayer.volume = 0;
        
        _gameplayMusicPlayer.Play();
        _menuMusicPlayer.Play();
    }

    public void ToggleMusic()
    {
        var toggle = PlayerPrefs.GetInt(PlayerPrefsValues.MUSIC_TOGGLE);
        
        if (toggle == 0)
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
    
    public void ToggleGameplay(bool isGameplay)
    {
        _isGameplay = isGameplay;
    }
    
    private void Update()
    {
        _crossFade = Mathf.SmoothDamp(_crossFade, _isGameplay ? 1 : 0, ref _velocity, _crossfadeSmooth);
        
        _gameplayMusicPlayer.volume = _crossFade;
        _menuMusicPlayer.volume = 1.0f - _crossFade;
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
