using UnityEngine;
using UnityEngine.UI;


public abstract class PopUpView : UIView<PopUpViewType>
{
    // ==================================================
    // Public Fields
    // ==================================================
    public override PopUpViewType Type => popUpType;

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private PopUpViewType popUpType;
    [SerializeField] private Button exitButton;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected virtual void OnValidate()
    {
        Debug.Assert(exitButton != null, "[PopUpView] Null exit button");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        UIButtonFactory.ClosePopUp<PopUpViewType>(exitButton, Host);
    }
}
