using UnityEngine;
using UnityEngine.UI;

public class SkipTutorialView : NewPopUpView
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

        yesButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.Tutorial));   // goes to the end of the tutorial
        noButton.onClick.AddListener(() => Manager.PopOverlay());
    }

    // base class
    protected override void SetInfo(IRuntimeData data) { }
}
