using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip placePlayerClip;
    [SerializeField] private AudioClip winClip;

    [SerializeField] private AudioClip transitionClip;

    // ==================================================
    // Private Fields
    // ==================================================
    private DataManager data => DataManager.Instance;

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

        Debug.Assert(transitionClip != null, "Transition clip not set in audio manager");
    }

    void Start()
    {
        sfxOn = (data.Settings.EffectsOn == 1);
        musicOn = (data.Settings.MusicOn == 1);

        if (sfxOn) TurnOnEffects();
        if (musicOn) TurnOnMusic();

        EventBus.Subscribe<UpdateSettingsEvent>(OnUpdateSettings);
    }

    void OnDestroy()
    {
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
        if (!sfxOn && data.Settings.EffectsOn == 1) TurnOnEffects();
        if (sfxOn && data.Settings.EffectsOn == 0) TurnOffEffects();

        if (!musicOn && data.Settings.MusicOn == 1) TurnOnMusic();
        if (musicOn && data.Settings.MusicOn == 0) TurnOnMusic();
    }

    // game play
    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        Debug.Log("place player sound");

        sfxSource.PlayOneShot(placePlayerClip);
    }

    private void OnWin(WinEvent e)
    {
        Debug.Log("win sound");

        sfxSource.PlayOneShot(winClip);
    }

    // UI
    private void OnTransition(TransitionEvent e)
    {
        Debug.Log("transition sound");

        sfxSource.PlayOneShot(transitionClip);
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
    }

    private void TurnOffMusic()
    {
        musicOn = false;
    }
}
