using UnityEngine;
using UnityEngine.UI;

public class TutorialViewContainer : BaseView
{
    // ==================================================
    // Inspector Fields
    // ==================================================

    [SerializeField] private Button skipButton;
    [SerializeField] private Button startPlayingButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void Awake()
    {
        // would potentially like to switch the skip button go to the end of the tutorial
        skipButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.Tutorial));
        startPlayingButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.GamePlay));
    }
}
