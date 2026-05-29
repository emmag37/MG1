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

        //yesButton.onClick.AddListener();
        //noButton.onClick.AddListener();
    }
}
