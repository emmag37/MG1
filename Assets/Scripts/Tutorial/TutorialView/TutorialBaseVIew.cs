using UnityEngine;
using UnityEngine.UI;

public class TutorialBaseView : BaseView
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

    void OnValidate()
    {
        Debug.Assert(skipButton != null, "[TutorialBaseView] Skip button is null");
        Debug.Assert(exitButton != null, "[TutorialBaseView] Exit button is null");
        Debug.Assert(startPlayingButton != null, "[TutorialBaseView] Start playing button is null");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        UIButtonFactory.Navigate<PopUpViewType>(skipButton, Host, PopUpViewType.SkipTutorial);
        UIButtonFactory.Navigate<BaseViewType>(exitButton, Host, BaseViewType.Home);
        UIButtonFactory.Navigate<BaseViewType>(startPlayingButton, Host, BaseViewType.GamePlay);
    }

    protected override void SetInfo(IUIData data) { }
}
