using UnityEngine;
using System.Collections.Generic;
using System;


public class AudioService : IAudio
{
    private AudioSettings settings;

    private AudioSource musicSource;
    private AudioSource sFXSource;

    private Dictionary<AudioType, AudioClipData> clipLookup = new Dictionary<AudioType, AudioClipData>();

    // constructor
    public AudioService(AudioSettings settings, AudioSource musicSource, AudioSource sFXSource)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));
        if (musicSource == null)
            throw new ArgumentNullException(nameof(musicSource));
        if (sFXSource == null)
            throw new ArgumentNullException(nameof(sFXSource));

        this.settings = settings;
        this.musicSource = musicSource;
        this.sFXSource = sFXSource;

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // load in all of the audio clips
        var audioClipObjects = Resources.LoadAll<AudioClipData>("AudioClips");
        if (audioClipObjects == null)
            throw new InvalidOperationException("[AudioService] No AudioClipData found under Resources/AudioClips");

        foreach (AudioClipData clip in audioClipObjects)
        {
            if (clip == null)
                throw new InvalidOperationException("[AudioService] Null audio clip loaded from Resources/AudioClips");
            clipLookup.Add(clip.type, clip);
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

    public void StopMusic()
    {
        Debug.Log("stop music");

        musicSource.Stop();
    }

    public void SetMusicOn(bool on) // used for continuous audio clips
    {
        Debug.Log($"pause/unpause music");

        if (settings.MusicOn == on) return;

        if (on)
            musicSource.Play();
        else
            musicSource.Pause();

        settings.MusicOn = on;
    }

    // sound effects functions
    public void PlaySoundEffect(AudioType audioType)
    {
        if (!settings.SFXOn) return;

        Debug.Log($"play effect: {audioType}");

        AudioClip clip = clipLookup[audioType].clip;
        sFXSource.PlayOneShot(clip);
    }

    public void SetEffectsOn(bool on)
    {
        settings.SFXOn = on;
    }

    // settings
    // done
    public AudioSettings GetSettings()
    {
        if (settings == null)
            throw new InvalidOperationException("[AudioService] Cannot return null settings from GetSettings");

        return settings;
    }
}

