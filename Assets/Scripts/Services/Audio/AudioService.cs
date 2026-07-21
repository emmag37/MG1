using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class AudioSettings
{
    public bool MusicOn;
    public bool SFXOn;
}

public class AudioService : IAudio
{
    private AudioSettings settings;

    private AudioSource musicSource;
    private AudioSource sFXSource;

    private Dictionary<AudioType, AudioClipData> clipLookup = new Dictionary<AudioType, AudioClipData>();

    // constructor
    public AudioService(AudioSettings settings, AudioSource musicSource, AudioSource sFXSource)
    {
        this.settings = settings;
        this.musicSource = musicSource;
        this.sFXSource = sFXSource;

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // load in all of the audio clips
        var audioClipObjects = Resources.LoadAll<AudioClipData>("AudioClips");
        foreach (AudioClipData data in audioClipObjects)
        {
            clipLookup.Add(data.type, data);
        }
    }

    // interface methods

    // background music functions
    public void PlayMusic(AudioType audioType)
    {
        Debug.Log($"play music: {audioType}");

        AudioClip clip = clipLookup[audioType].clip;
        musicSource.clip = clip;

        if (settings.MusicOn)
            musicSource.Play();
    }
    public void StopMusic(AudioType audioType)
    {
        Debug.Log($"stop music: {audioType}");
    }

    // sound effects functions
    public void PlaySoundEffect(AudioType audioType)
    {
        if (!settings.SFXOn) return;

        Debug.Log($"play effect: {audioType}");

        AudioClip clip = clipLookup[audioType].clip;
        sFXSource.PlayOneShot(clip);
    }

    // settings
    public AudioSettings GetSettings()
    {
        return settings;
    }
}

