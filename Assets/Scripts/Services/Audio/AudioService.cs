using UnityEngine;
using System.Collections.Generic;


public class AudioService : IAudio
{
    private AudioSource musicSource;
    private AudioSource sFXSource;

    private Dictionary<AudioType, AudioClipData> clipLookup = new Dictionary<AudioType, AudioClipData>();

    // constructor
    public AudioService(AudioSource musicSource, AudioSource sFXSource)
    {
        this.musicSource = musicSource;
        this.sFXSource = sFXSource;

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
    }
    public void StopMusic(AudioType audioType)
    {
        Debug.Log($"stop music: {audioType}");
    }

    // sound effects functions
    public void PlaySoundEffect(AudioType audioType)
    {
        Debug.Log($"play effect: {audioType}");

        AudioClip clip = clipLookup[audioType].clip;
        sFXSource.PlayOneShot(clip);
    }
}

