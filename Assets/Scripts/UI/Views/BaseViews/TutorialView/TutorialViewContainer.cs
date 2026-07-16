using UnityEngine;
using UnityEngine.UI;

public class TutorialViewContainer : NewBaseView
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
        skipButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.SkipTutorial));
        exitButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.Home));

        startPlayingButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.GamePlay));
    }

    protected override void SetInfo(IRuntimeData data) { }
}
