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
        UIButtonFactory.Navigate<PopUpViewType>(skipButton, Host, PopUpViewType.SkipTutorial);
        UIButtonFactory.Navigate<BaseViewType>(exitButton, Host, BaseViewType.Home);
        UIButtonFactory.Navigate<BaseViewType>(startPlayingButton, Host, BaseViewType.GamePlay);
    }

    protected override void SetInfo(IUIData data) { }
}
