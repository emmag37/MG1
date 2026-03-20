using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the home screen.
/// </summary>
public class HomeView: BaseView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button profileButton;
    [SerializeField] private Button scoreHistoryButton;
    [SerializeField] private Button playButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(profileButton != null, "Profile button not set in home view");
        Debug.Assert(scoreHistoryButton != null, "Score history button not set in home view");
        Debug.Assert(playButton != null, "Play button not set in home view");
    }


    // ==================================================
    // Button Methods
    // ==================================================

    public void OnProfileClicked()
    {
        UI.PushOverlay(PopUpViewType.Profile);
    }

    public void OnScoreHistoryClicked()
    {
        UI.PushOverlay(PopUpViewType.ScoreHistory);
    }

    public void OnPlayClicked()
    {
        UI.ShowView(BaseViewType.GamePlay);
    }
}
