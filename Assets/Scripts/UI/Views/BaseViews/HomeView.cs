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
    // Private Fields
    // ==================================================
    private ProfileData profile;

    UIButton<PopUpViewType> profileUIButton;
    UIButton<PopUpViewType> settingsUIButton;
    UIButton<BaseViewType> playUIButton;


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
        /*
        profileButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.Profile));
        settingsButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.Settings));

        playButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.GamePlay));
        */

        profileUIButton = new UIButton<PopUpViewType>(Host, profileButton, PopUpViewType.Profile);
        settingsUIButton = new UIButton<PopUpViewType>(Host, settingsButton, PopUpViewType.Settings);
        playUIButton = new UIButton<BaseViewType>(Host, playButton, BaseViewType.GamePlay);

    }

    // ==================================================
    // Public Methods
    // ==================================================

    protected override void InitializeData(IUIData initData)
    {
        if (initData is not ProfileData profile)
        {
            Debug.Log($"data passed to initialize home view is not profile, type: {initData?.GetType().Name}");
            return;
        }

        this.profile = profile;
    }

    protected override void SetInfo(IUIData data)
    {
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)profile.Avatar);
    }
}
