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
    // Unity Lifecycle
    // ==================================================

    protected override void Awake()
    {
        base.Awake();

        yesButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.Tutorial));   // goes to the end of the tutorial
        noButton.onClick.AddListener(() => Host.PopView<PopUpViewType>());
    }

    // base class
    protected override void InitializeData(IUIData initData) { }
    protected override void SetInfo(IUIData data) { }
}
