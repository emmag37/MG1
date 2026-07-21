using UnityEngine;

public interface IAudio
{
    // background music functions
    public void PlayMusic(AudioType audioType);
    public void StopMusic(AudioType audioType);

    // sound effects functions
    public void PlaySoundEffect(AudioType audioType);
}
