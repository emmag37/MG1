using UnityEngine;


public interface IAudio
{
    // for continuous audio - only one active at a time
    void PlayMusic(AudioType audioType);
    void StopMusic();
    void SetMusicOn(bool on);

    // for one shot audio
    void PlaySoundEffect(AudioType audioType);
    void SetEffectsOn(bool on);

    // settings
    AudioSettings GetSettings();
}
