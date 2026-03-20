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

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(homeButton != null, "Home button not set in pause view");
        Debug.Assert(restartButton != null, "Restart button not set in pause view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnHomeClicked()
    {
        UI.ShowView(BaseViewType.Home);
    }

    public void OnReplayClicked()
    {
        UI.ShowView(BaseViewType.GamePlay);
    }
}
