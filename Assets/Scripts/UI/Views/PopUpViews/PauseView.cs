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

        homeButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.Home));
        restartButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.RestartGame));

        // re-implement
        //musicSlider.onValueChanged.AddListener((value) => Manager.UpdateMusicOn((int)value));
        //sfxSlider.onValueChanged.AddListener((value) => Manager.UpdateSFXOn((int)value));
    }


    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        if (data is not IUserSettings settings)
        {
            Debug.Log($"data passed to pause view is not user settings, type: {data?.GetType().Name}");
            return;
        }

        musicSlider.value = settings.MusicOn ? 1 : 0;
        sfxSlider.value = settings.SFXOn ? 1 : 0;
    }
}
