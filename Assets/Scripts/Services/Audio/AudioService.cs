using UnityEngine;


public class AudioService : IAudio
{
    private AudioSource musicSource;
    private AudioSource sFXSource;

    // constructor
    public AudioService(AudioSource musicSource, AudioSource sFXSource)
    {
        this.musicSource = musicSource;
        this.sFXSource = sFXSource;

        // initialize the scriptable objects
    }

    // background music functions
    public void PlayMusic(int musicID)
    {
        Debug.Log("play music");
    }
    public void StopMusic(int musicID)
    {
        Debug.Log("stop music");
    }

    // sound effects functions
    public void PlaySoundEffect(int effectID)
    {
        Debug.Log("play effect");
    }
}

