using UnityEngine;
using UnityEngine.UI;


public class PauseView : PopUpView
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

        Debug.Assert(homeButton != null, "[PauseView] Null home button");
        Debug.Assert(restartButton != null, "[PauseView] Null restart button");

        Debug.Assert(musicSlider != null, "[PauseView] Null music slider");
        Debug.Assert(sfxSlider != null, "[PauseView] Null sfx slider");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        audioService = ServiceLocator.Get<IAudio>();
        vibrationService = ServiceLocator.Get<IVibration>();

        UIButtonFactory.Navigate<BaseViewType>(homeButton, Host, BaseViewType.Home, (int)AudioType.Button);
        UIButtonFactory.Navigate<PopUpViewType>(restartButton, Host, PopUpViewType.RestartGame, (int)AudioType.Button);

        new UIToggle(musicSlider, audioService.SetMusicOn, (int)AudioType.Button);
        new UIToggle(sfxSlider, audioService.SetEffectsOn, (int)AudioType.Button);
        new UIToggle(vibrateSlider, vibrationService.SetVibrationOn, (int)AudioType.Button);
    }

    protected override void SetInfo(IUIData data)
    {
        AudioSettings audioSettings = audioService.GetSettings();
        bool vibrateOn = vibrationService.GetSettings();

        musicSlider.SetValueWithoutNotify(audioSettings.MusicOn ? 1 : 0);
        sfxSlider.SetValueWithoutNotify(audioSettings.SFXOn ? 1 : 0);

        vibrateSlider.SetValueWithoutNotify(vibrateOn ? 1 : 0);
    }
}
