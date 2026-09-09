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

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(yesButton != null, "[SkipTutorialView] Null yes button");
        Debug.Assert(noButton != null, "[SkipTutorialView] Null no button");
    }
    

    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        UIButtonFactory.Navigate<BaseViewType>(yesButton, Host, BaseViewType.Tutorial);
        UIButtonFactory.ClosePopUp<PopUpViewType>(noButton, Host);
    }

    protected override void SetInfo(IUIData data) { }       // empty func to satisfy required inheritance
}
