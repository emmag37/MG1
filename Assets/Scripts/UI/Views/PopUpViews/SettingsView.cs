using UnityEngine;
using UnityEngine.UI;


// todo: reimplement update music and sfx

public class SettingsView: PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button termCondButton;
    [SerializeField] private Button privacyPolButton;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // ==================================================
    // Private Fields
    // ==================================================

    private SettingsData settings;

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


        musicSlider.onValueChanged.AddListener((value) => UpdateMusic(value > 0));   // cast to bool, 1 for on, 0 for off
        sfxSlider.onValueChanged.AddListener((value) => UpdateSFX(value > 0));
    }


    // ==================================================
    // Protected Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        if (data is not SettingsData settings)
        {
            Debug.Log($"data passed to settings view is not settings, type: {data?.GetType().Name}");
            return;
        }

        this.settings = settings;

        musicSlider.value = settings.MusicOn ? 1 : 0;
        sfxSlider.value = settings.SFXOn ? 1 : 0;
    }

    // ==================================================
    // Private Methods
    // ==================================================

    private void UpdateMusic(bool on)
    {
        settings.MusicOn = on;
        Host.UpdateData(settings);
    }

    private void UpdateSFX(bool on)
    {
        settings.SFXOn = on;
        Host.UpdateData(settings);
    }
}
