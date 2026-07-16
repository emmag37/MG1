using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the home screen.
/// </summary>
public class HomeView : BaseView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button profileButton;
    [SerializeField] private Image avatarImage;

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

    protected override void SetInfo(IRuntimeData data)
    {
        if (data is not IUserSettings settings)
        {
            Debug.Log($"data passed to home view is not user settings, type: {data?.GetType().Name}");
            return;
        }

        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)settings.Avatar);
    }
}
