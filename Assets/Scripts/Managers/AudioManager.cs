using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
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
