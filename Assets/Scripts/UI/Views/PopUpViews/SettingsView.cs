using UnityEngine;
using UnityEngine.UI;

public class SettingsView: NewPopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button termCondButton;
    [SerializeField] private Button privacyPolButton;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

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

        // link for terms and conditions
        // link for privacy policy

        musicSlider.onValueChanged.AddListener((value) => Manager.UpdateMusicOn((int)value));
        sfxSlider.onValueChanged.AddListener((value) => Manager.UpdateSFXOn((int)value));
    }


    // ==================================================
    // Protected Methods
    // ==================================================

    protected override void SetInfo(IRuntimeData data)
    {
        if (data is not IUserSettings settings)
        {
            Debug.Log($"data passed to settings view is not user settings, type: {data?.GetType().Name}");
            return;
        }

        musicSlider.value = settings.MusicOn ? 1 : 0;
        sfxSlider.value = settings.SFXOn ? 1 : 0;
    }
}
