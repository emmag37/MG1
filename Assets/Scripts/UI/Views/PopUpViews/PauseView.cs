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

        UIButtonFactory.Navigate<BaseViewType>(homeButton, Host, BaseViewType.Home);
        UIButtonFactory.Navigate<PopUpViewType>(restartButton, Host, PopUpViewType.RestartGame);

        new UIToggle(musicSlider, audioService.SetMusicOn);
        new UIToggle(sfxSlider, audioService.SetEffectsOn);
        new UIToggle(vibrateSlider, vibrationService.SetVibrationOn);
    }


    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        Debug.Log("set info");

        AudioSettings audioSettings = audioService.GetSettings();
        bool vibrateOn = vibrationService.GetSettings();

        musicSlider.SetValueWithoutNotify(audioSettings.MusicOn ? 1 : 0);
        sfxSlider.SetValueWithoutNotify(audioSettings.SFXOn ? 1 : 0);

        vibrateSlider.SetValueWithoutNotify(vibrateOn ? 1 : 0);
    }
}
