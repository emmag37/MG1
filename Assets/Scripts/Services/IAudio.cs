using UnityEngine;

public interface IAudio
{
    // background music functions
    public void PlayMusic(int musicID);
    public void StopMusic(int musicID);

    // sound effects functions
    public void PlaySoundEffect(int effectID);
}
