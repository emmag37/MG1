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
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        skipButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.SkipTutorial));
        exitButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.Home));

        startPlayingButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.GamePlay));
    }

    protected override void InitializeData(IUIData initData) { }
    protected override void SetInfo(IUIData data) { }
}
