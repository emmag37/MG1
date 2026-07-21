using UnityEngine;
using UnityEngine.UI;

// todo: reimplement update music and sfx

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

    // ==================================================
    // Private Fields
    // ==================================================

    private IAudio audioService;


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

        homeButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.Home));
        restartButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.RestartGame));

        musicSlider.onValueChanged.AddListener((value) => UpdateMusic(value > 0));   // cast to bool, 1 for on, 0 for off
        sfxSlider.onValueChanged.AddListener((value) => UpdateSFX(value > 0));
    }


    // ==================================================
    // Base Class Methods
    // ==================================================

    public override void Show(IUIData data = null)
    {
        base.Show(data);

        AudioSettings audioSettings = audioService.GetSettings();

        musicSlider.value = audioSettings.MusicOn ? 1 : 0;
        sfxSlider.value = audioSettings.SFXOn ? 1 : 0;
    }

    protected override void SetInfo(IUIData data) { }

    // ==================================================
    // Private Methods
    // ==================================================

    private void UpdateMusic(bool on)
    {
        AudioSettings audioSettings = audioService.GetSettings();
        audioSettings.MusicOn = on;
    }

    private void UpdateSFX(bool on)
    {
        AudioSettings audioSettings = audioService.GetSettings();
        audioSettings.SFXOn = on;
    }
}
