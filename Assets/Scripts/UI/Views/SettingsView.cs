using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the settings menu.
/// </summary>
public class SettingsView: UIView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(homeButton != null, "Home button not set in settings view");
        Debug.Assert(restartButton != null, "Restart button not set in settings view");
        Debug.Assert(exitButton != null, "Exit button not set in settings view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnHomeClicked()
    {
        UIManager.Instance.LaunchHomeScreen(true);
    }

    public void OnReplayClicked()
    {
        UIManager.Instance.LaunchNewGame(true);
    }

    public void OnExitClicked()
    {
        UIManager.Instance.CloseSettings();
    }
}
