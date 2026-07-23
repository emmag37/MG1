using UnityEngine;
using UnityEngine.UI;

public class TutorialViewContainer : BaseView
{
    // ==================================================
    // Inspector Fields
    // ==================================================

    [SerializeField] private Button skipButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button startPlayingButton;

    // ==================================================
    // Private Fields
    // ==================================================

    private UIButton skipUIButton;
    private UIButton exitUIButton;
    private UIButton startPlayingUIButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        skipUIButton = UIButtonFactory.Navigate<PopUpViewType>(skipButton, Host, PopUpViewType.SkipTutorial);
        exitUIButton = UIButtonFactory.Navigate<BaseViewType>(exitButton, Host, BaseViewType.Home);
        startPlayingUIButton = UIButtonFactory.Navigate<BaseViewType>(startPlayingButton, Host, BaseViewType.GamePlay);
    }

    protected override void InitializeData(IUIData initData) { }
    protected override void SetInfo(IUIData data) { }
}
