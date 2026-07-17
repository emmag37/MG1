using UnityEngine;


// todo: re-implement settings updates

public class AudioManager : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip placePlayerClip;
    [SerializeField] private AudioClip pickupPlayerClip;

    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip gameOverClip;

    [SerializeField] private AudioClip buttonClip;
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

        Debug.Assert(buttonClip != null, "Transition clip not set in audio manager");
    }

    void OnEnable()
    {
        // game state events
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
        EventBus.Subscribe<StartGameEvent>(OnStartGame);

        // game play events
        EventBus.Subscribe<PlayerDraggingEvent>(OnPlayerDrag);
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Subscribe<WinEvent>(OnWin);
    }

    void OnDisable()
    {
        // game state events
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);

        // game play events
        EventBus.Unsubscribe<PlayerDraggingEvent>(OnPlayerDrag);
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

    public void Play()
    {
        if (musicOn) musicSource.Play();
    }


    // ================================
    // Event Handlers
    // ================================

    public void HandleSettingsUpdate(bool music, bool sFX)
    {
        musicOn = music;
        sfxOn = sFX;

        if (musicOn)
            musicSource.Play();
        else
            musicSource.Stop();
    }

    
    // sound effects
    public void HandleButtonPressed()
    {
        if (sfxOn)
            sfxSource.PlayOneShot(buttonClip);
    }

    public void HandleTransition()
    {
        if (sfxOn)
            sfxSource.PlayOneShot(transitionClip);
    }

    private void OnPlayerDrag(PlayerDraggingEvent e)
    {
        if (sfxOn)
            sfxSource.PlayOneShot(pickupPlayerClip);
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
}
