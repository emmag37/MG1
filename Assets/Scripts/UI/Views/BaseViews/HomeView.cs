using UnityEngine;
using UnityEngine.UI;


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
    ISpriteDatabase spriteDatabase;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(profileButton != null, "[HomeView] Null profile button");
        Debug.Assert(settingsButton != null, "[HomeView] Null settings button");
        Debug.Assert(playButton != null, "[HomeView] Null play button");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        spriteDatabase = ServiceLocator.Get<ISpriteDatabase>();

        UIButtonFactory.Navigate<PopUpViewType>(profileButton, Host, PopUpViewType.Profile, (int)AudioType.Button);
        UIButtonFactory.Navigate<PopUpViewType>(settingsButton, Host, PopUpViewType.Settings, (int)AudioType.Button);
        UIButtonFactory.Navigate<BaseViewType>(playButton, Host, BaseViewType.GamePlay, (int)AudioType.Button);
    }

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.LogError($"Data type mismatch, wanted ProfileData, recieved {data?.GetType().Name}");
            return;
        }

        avatarImage.sprite = spriteDatabase.GetSprite((int)profile.Avatar);
    }
}
