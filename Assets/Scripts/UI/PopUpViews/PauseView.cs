using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the settings menu.
/// </summary>
public class PauseView: PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button helpButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(homeButton != null, "Home button not set in pause view");
        Debug.Assert(restartButton != null, "Restart button not set in pause view");
        Debug.Assert(helpButton != null, "Help button not set in pause view");
    }

    protected override void Awake()
    {
        base.Awake();

        homeButton.onClick.AddListener(() => UI.ShowView(BaseViewType.Home));
        restartButton.onClick.AddListener(() => UI.ShowView(BaseViewType.GamePlay));
        helpButton.onClick.AddListener(() => UI.PushOverlay(PopUpViewType.Tutorial1));
    }
}
