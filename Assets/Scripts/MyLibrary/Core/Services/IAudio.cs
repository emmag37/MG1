
public interface IAudio
{
    // for continuous audio - only one active at a time
    void PlayMusic(int audioID);
    void StopMusic();
    void SetMusicOn(bool on);

    // for one shot audio
    void PlaySoundEffect(int audioID);
    void SetEffectsOn(bool on);

    // settings
    AudioSettings GetSettings();
}
