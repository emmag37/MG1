using UnityEngine;
using UnityEngine.UI;

public class RestartGameView : PopUpView
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

        Debug.Assert(yesButton != null, "[RestartGameView] Null yes button");
        Debug.Assert(noButton != null, "[RestartGameView] Null no button");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        UIButtonFactory.Navigate<BaseViewType>(yesButton, Host, BaseViewType.GamePlay, (int)AudioType.Button);
        UIButtonFactory.ClosePopUp<PopUpViewType>(noButton, Host, (int)AudioType.Button);
    }
    
    protected override void SetInfo(IUIData data) { }       // empty func to satisfy required inheritance
}
