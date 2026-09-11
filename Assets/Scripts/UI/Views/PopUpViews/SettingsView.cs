using UnityEngine;
using UnityEngine.UI;


public class SettingsView : PopUpView
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

        Debug.Assert(termCondButton != null, "[SettingsView] Null terms and conditions button");
        Debug.Assert(privacyPolButton != null, "[SettingsView] Null privacy policy button");

        Debug.Assert(musicSlider != null, "[SettingsView] Null music slider");
        Debug.Assert(sfxSlider != null, "[SettingsView] Null sfx slider");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        audioService = ServiceLocator.Get<IAudio>();
        vibrationService = ServiceLocator.Get<IVibration>();

        // link for terms and conditions
        // link for privacy policy

        new UIToggle(musicSlider, audioService.SetMusicOn, (int)AudioType.Button);
        new UIToggle(sfxSlider, audioService.SetEffectsOn, (int)AudioType.Button);
        new UIToggle(vibrateSlider, vibrationService.SetVibrationOn, (int)AudioType.Button);
    }

    protected override void SetInfo(IUIData data)                           // ignore data parameter
    {
        AudioSettings audioSettings = audioService.GetSettings();
        bool vibrateOn = vibrationService.GetSettings();

        // on set, do not play sound
        musicSlider.SetValueWithoutNotify(audioSettings.MusicOn ? 1 : 0);
        sfxSlider.SetValueWithoutNotify(audioSettings.SFXOn ? 1 : 0);

        vibrateSlider.SetValueWithoutNotify(vibrateOn ? 1 : 0);
    }
}
