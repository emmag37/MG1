using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip placePlayerClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip transitionClip;

    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip UIMusic;

    // ==================================================
    // Private Fields
    // ==================================================
    private bool sfxOn;
    private bool musicOn;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(sfxSource != null, "SFX source not set in audio manager");

        Debug.Assert(placePlayerClip != null, "Place player clip not set in audio manager");
        Debug.Assert(winClip != null, "Win clip not set in audio manager");
        Debug.Assert(gameOverClip != null, "Game over clip not set in audio manager");

        Debug.Assert(transitionClip != null, "Transition clip not set in audio manager");
    }

    void Start()
    {
        // initialize the clip to ui
        musicSource.clip = UIMusic;
        musicSource.loop = true;     
        musicSource.playOnAwake = false;

        // need to maintain the current audio clip in case music turns on/off
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);

        // initialize the settings
        //sfxOn = (data.Settings.EffectsOn == 1);
        //musicOn = (data.Settings.MusicOn == 1);

        if (sfxOn) TurnOnEffects();
        if (musicOn) TurnOnMusic();

        EventBus.Subscribe<UpdateSettingsEvent>(OnUpdateSettings);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);

        if (sfxOn) TurnOffEffects();
        if (musicOn) TurnOffMusic();

        EventBus.Unsubscribe<UpdateSettingsEvent>(OnUpdateSettings);
    }


    // ================================
    // Event Handlers
    // ================================

    // settings
    private void OnUpdateSettings(UpdateSettingsEvent e)
    {
        /*
        if (!sfxOn && data.Settings.EffectsOn == 1) TurnOnEffects();
        if (sfxOn && data.Settings.EffectsOn == 0) TurnOffEffects();

        if (!musicOn && data.Settings.MusicOn == 1) TurnOnMusic();
        if (musicOn && data.Settings.MusicOn == 0) TurnOffMusic();
        */
    }

    // sound effects
    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        sfxSource.PlayOneShot(placePlayerClip);
    }

    private void OnWin(WinEvent e)
    {
        sfxSource.PlayOneShot(winClip);
    }

    private void OnTransition(TransitionEvent e)
    {
        sfxSource.PlayOneShot(transitionClip);
    }




    // music
    private void OnGameOver(GameOverEvent e)
    {
        musicSource.Stop();

        musicSource.clip = UIMusic;
        if (musicOn)
        {
            sfxSource.PlayOneShot(gameOverClip);

            musicSource.Play();
        }
    }

    private void OnStartGame(StartGameEvent e)
    {
        musicSource.Stop();

        musicSource.clip = gameMusic;
        if (musicOn) musicSource.Play();
    }

    private void OnExitGame(ExitGameEvent e)
    {
        musicSource.Stop();

        musicSource.clip = UIMusic;
        if (musicOn) musicSource.Play();
    }


    // ================================
    // Private Methods
    // ================================

    private void TurnOnEffects()
    {
        sfxOn = true;

        // game play events
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Subscribe<WinEvent>(OnWin);

        // UI events
        EventBus.Subscribe<TransitionEvent>(OnTransition);
    }

    private void TurnOffEffects()
    {
        sfxOn = false;

        // game play events
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Unsubscribe<WinEvent>(OnWin);

        // UI events
        EventBus.Unsubscribe<TransitionEvent>(OnTransition);
    }

    private void TurnOnMusic()
    {
        musicOn = true;

        musicSource.Play();
    }

    private void TurnOffMusic()
    {
        musicOn = false;

        musicSource.Stop();
    }
}
