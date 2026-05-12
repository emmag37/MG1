using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the home screen.
/// </summary>
public class HomeView : BaseView<IUserSettings>
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button profileButton;
    [SerializeField] private Button settingsButton;

    [SerializeField] private Button playButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(profileButton != null, "Profile button not set in home view");
        Debug.Assert(settingsButton != null, "Settings button not set in home view");
        Debug.Assert(playButton != null, "Play button not set in home view");
    }

    void Awake()
    {
        profileButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.Profile));
        settingsButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.Settings));

        playButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.GamePlay));
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(IUserSettings data)
    {
        profileButton.image.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        base.Show(data);
    }

    public override void UpdateView(IUserSettings data)
    {
        profileButton.image.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        base.UpdateView(data);
    }
}
