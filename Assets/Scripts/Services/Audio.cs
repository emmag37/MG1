using UnityEngine;

public class Audio : IAudio
{
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
