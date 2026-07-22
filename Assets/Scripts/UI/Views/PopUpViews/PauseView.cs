using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// UI view for the settings menu.
/// </summary>
public class PauseView: PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private Slider vibrateSlider;

    // ==================================================
    // Private Fields
    // ==================================================

    private IAudio audioService;
    private IVibration vibrationService;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(homeButton != null, "Home button not set in pause view");
        Debug.Assert(restartButton != null, "Restart button not set in pause view");

        Debug.Assert(musicSlider != null, "Music slider not set in pause view");
        Debug.Assert(sfxSlider != null, "Effects slider not set in pause view");
    }

    protected override void Awake()
    {
        base.Awake();

        audioService = ServiceLocator.Get<IAudio>();
        vibrationService = ServiceLocator.Get<IVibration>();

        homeButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.Home));
        restartButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.RestartGame));

        musicSlider.onValueChanged.AddListener((value) => audioService.SetMusicOn(value > 0));   // cast to bool, 1 for on, 0 for off
        sfxSlider.onValueChanged.AddListener((value) => audioService.SetEffectsOn(value > 0));

        vibrateSlider.onValueChanged.AddListener((value) => vibrationService.SetVibrationOn(value > 0));
    }


    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void InitializeData(IUIData initData) { }

    protected override void SetInfo(IUIData data)
    {
        Debug.Log("set info");

        AudioSettings audioSettings = audioService.GetSettings();
        bool vibrateOn = vibrationService.GetSettings();

        musicSlider.value = audioSettings.MusicOn ? 1 : 0;
        sfxSlider.value = audioSettings.SFXOn ? 1 : 0;

        vibrateSlider.value = vibrateOn ? 1 : 0;
    }

    // ==================================================
    // Private Methods
    // ==================================================

    /*
    private void UpdateMusic(bool on)
    {
        audioService.SetMusicOn(on);
    }

    private void UpdateSFX(bool on)
    {
        audioService.SetEffectsOn(on);
    }
    */
}
