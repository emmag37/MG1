using UnityEngine;
using UnityEngine.UI;


public class SettingsView: PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button termCondButton;
    [SerializeField] private Button privacyPolButton;

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

        Debug.Assert(termCondButton != null, "Terms and conditions button not set in settings view");
        Debug.Assert(privacyPolButton != null, "Privacy policy button not set in settings view");

        Debug.Assert(musicSlider != null, "Music slider not set in settings view");
        Debug.Assert(sfxSlider != null, "Effects slider not set in settings view");
    }

    protected override void Awake()
    {
        base.Awake();

        audioService = ServiceLocator.Get<IAudio>();
        vibrationService = ServiceLocator.Get<IVibration>();

        // link for terms and conditions
        // link for privacy policy

        musicSlider.onValueChanged.AddListener((value) => audioService.SetMusicOn(value > 0));   // cast to bool, 1 for on, 0 for off
        sfxSlider.onValueChanged.AddListener((value) => audioService.SetEffectsOn(value > 0));

        vibrateSlider.onValueChanged.AddListener((value) => vibrationService.SetVibrationOn(value > 0));
    }


    // ==================================================
    // Protected Methods
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
