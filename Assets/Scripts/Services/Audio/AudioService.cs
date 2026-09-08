using UnityEngine;
using System.Collections.Generic;
using System;


public class AudioService : IAudio
{
    // ==================================================
    // Private Fields
    // ==================================================
    private AudioSettings settings;

    private AudioSource musicSource;
    private AudioSource sFXSource;

    private Dictionary<AudioType, AudioClipData> clipLookup = new Dictionary<AudioType, AudioClipData>();


    // ==================================================
    // Constructor
    // ==================================================

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

    // ==================================================
    // Interface Methods - Background Music
    // ==================================================

    public void PlayMusic(AudioType audioType)
    {
        if (!clipLookup.ContainsKey(audioType))
        {
            Debug.LogError($"[AudioService] No clip found with type {audioType}");
            return;
        }

        musicSource.clip = clipLookup[audioType].clip;
        if (settings.MusicOn)
            musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // used for continuous audio clips
    public void SetMusicOn(bool on)
    {
        if (settings.MusicOn == on) return;

        if (on)
            musicSource.Play();
        else
            musicSource.Pause();

        settings.MusicOn = on;
    }


    // ==================================================
    // Interface Methods - Sound Effects
    // ==================================================

    public void PlaySoundEffect(AudioType audioType)
    {
        if (!clipLookup.ContainsKey(audioType))
        {
            Debug.LogError($"[AudioService] No clip found with type {audioType}");
            return;
        }

        if (settings.SFXOn)
            sFXSource.PlayOneShot(clipLookup[audioType].clip);
    }

    public void SetEffectsOn(bool on)
    {
        settings.SFXOn = on;
    }

    // ==================================================
    // Interface Methods - Settings
    // ==================================================

    public AudioSettings GetSettings()
    {
        if (settings == null)
            throw new InvalidOperationException("[AudioService] Cannot return null settings from GetSettings");

        return settings;
    }
}

