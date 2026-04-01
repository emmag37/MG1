using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the settings menu.
/// </summary>
public class PauseView: PopUpView<UserSettings>
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button helpButton;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(homeButton != null, "Home button not set in pause view");
        Debug.Assert(restartButton != null, "Restart button not set in pause view");
        Debug.Assert(helpButton != null, "Help button not set in pause view");

        Debug.Assert(musicSlider != null, "Music slider not set in pause view");
        Debug.Assert(effectsSlider != null, "Effects slider not set in pause view");
    }

    protected override void Awake()
    {
        base.Awake();

        homeButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.Home));
        restartButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.GamePlay));

        helpButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.Tutorial));

        musicSlider.onValueChanged.AddListener((value) => Manager.UpdateMusicOn((int)value));
        effectsSlider.onValueChanged.AddListener((value) => Manager.UpdateEffectsOn((int)value));
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(UserSettings data)
    {
        musicSlider.value = data.MusicOn;
        effectsSlider.value = data.EffectsOn;

        base.Show(data);
    }
}
