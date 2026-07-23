using UnityEngine;
using UnityEngine.UI;

public class SkipTutorialView : PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    // ==================================================
    // Private Fields
    // ==================================================

    private UIButton yesUIButton;
    private UIButton noUIButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void Awake()
    {
        base.Awake();

        yesUIButton = UIButtonFactory.Navigate<BaseViewType>(yesButton, Host, BaseViewType.Tutorial);
        noUIButton = UIButtonFactory.ClosePopUp<PopUpViewType>(noButton, Host);
    }

    // base class
    protected override void InitializeData(IUIData initData) { }
    protected override void SetInfo(IUIData data) { }
}
