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
        // add listener for skip button
    }
}
