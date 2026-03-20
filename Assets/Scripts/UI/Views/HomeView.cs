using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the home screen.
/// </summary>
public class HomeView: BaseUIView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button profileButton;
    [SerializeField] private Button playButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(profileButton != null, "Profile button not set in home view");
        Debug.Assert(playButton != null, "Play button not set in home view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnProfileClicked()
    {
        UI.PushOverlay(PopUpViewType.Profile);
    }

    public void OnPlayClicked()
    {
        UI.ShowView(BaseViewType.GamePlay);
    }
}
