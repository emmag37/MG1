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

    void OnEnable()
    {
        // game state events
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);

        // game play events
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Subscribe<WinEvent>(OnWin);
    }

    void OnDisable()
    {
        // game state events
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);

        // game play events
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Unsubscribe<WinEvent>(OnWin);
    }

    // ==================================================
    // Initializers
    // ==================================================

    public void Initialize(bool musicOn, bool sfxOn)
    {
        this.musicOn = musicOn;
        this.sfxOn = sfxOn;

        musicSource.clip = UIMusic;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }


    // ================================
    // Event Handlers
    // ================================

    // settings
	public void HandleMusicUpdate(bool on)
    {
        musicOn = on;

        if (musicOn)
            musicSource.Play();
        else
            musicSource.Stop();
    }

    public void HandleSFXUpdate(bool on)
    {
        sfxOn = on;
    }

    // sound effects
    public void HandleButtonPressed()
    {
        if (sfxOn)
            sfxSource.PlayOneShot(transitionClip);
    }

    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        if (sfxOn)
            sfxSource.PlayOneShot(placePlayerClip);
    }

    private void OnWin(WinEvent e)
    {
        if (sfxOn)
            sfxSource.PlayOneShot(winClip);
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
}
