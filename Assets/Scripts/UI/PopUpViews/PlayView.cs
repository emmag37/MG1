using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game play scene.
/// </summary>
public class PlayView: BaseView
{


    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button pauseButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(pauseButton != null, "Pause button not set in play view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnPauseClicked()
    {
        UI.PushOverlay(PopUpViewType.Pause);
    }
}
