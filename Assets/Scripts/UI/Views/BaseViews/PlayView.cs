using UnityEngine;
using UnityEngine.UI;


public class PlayView : BaseView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button pauseButton;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(pauseButton != null, "[PlayView] Null pause button");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        UIButtonFactory.Navigate<PopUpViewType>(pauseButton, Host, PopUpViewType.Pause, (int)AudioType.Button);
    }

    protected override void SetInfo(IUIData data) { }   // empty method for required inheritance
}