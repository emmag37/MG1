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

    protected override void Awake()
    {
        base.Awake();

        yesButton.onClick.AddListener(() => Host.PushView<BaseViewType>(BaseViewType.GamePlay));   // should close whole stack, show a new game
        noButton.onClick.AddListener(() => Host.PopView<PopUpViewType>()); // close the whole thing - don't remember the function usage
    }

    // base class
    protected override void SetInfo(IRuntimeData data) { }
}
