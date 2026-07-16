using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the settings menu.
/// </summary>
public class PauseView: PopUpView<IUserSettings>
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

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

        homeButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.Home));
        restartButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.RestartGame));

        musicSlider.onValueChanged.AddListener((value) => Manager.UpdateMusicOn((int)value));
        sfxSlider.onValueChanged.AddListener((value) => Manager.UpdateSFXOn((int)value));
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(IUserSettings data)
    {
        musicSlider.value = data.MusicOn ? 1 : 0;
        sfxSlider.value = data.SFXOn ? 1 : 0;

        base.Show(data);
    }
}
