using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game play scene.
/// </summary>
public class PlayView: UIView
{


    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button settingsButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(settingsButton != null, "Settings button not set in play view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnSettingsClicked()
    {
        UI.PushOverlay(ViewType.Settings);
    }
}
