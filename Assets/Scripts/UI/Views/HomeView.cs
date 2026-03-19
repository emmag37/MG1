using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the home screen.
/// </summary>
public class HomeView: UIView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button playButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(playButton != null, "Play button not set in home view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnPlayClicked()
    {
        UIManager.Instance.LaunchNewGame(false);
    }
}
