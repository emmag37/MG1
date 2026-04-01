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

    void Awake()
    {
        // need to give this the actual settings data
        pauseButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.Pause));
    }
}
