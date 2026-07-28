using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game over screen.
/// </summary>
public class GameOverView : BaseView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button replayButton;

    [SerializeField] private Text gameScoreText;
    [SerializeField] private Text highScoreText;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(settingsButton != null, "Settings button not set in game over view");
        Debug.Assert(homeButton != null, "Home button not set in game over view");
        Debug.Assert(replayButton != null, "Replay button not set in game over view");

        Debug.Assert(gameScoreText != null, "Game score text not set in game over view");
        Debug.Assert(highScoreText != null, "High score text not set in game over view");
    }

    void Awake()
    {
        // put these into some sort of list to dispose of later
        UIButtonFactory.Navigate<PopUpViewType>(settingsButton, Host, PopUpViewType.Settings);
        UIButtonFactory.Navigate<BaseViewType>(homeButton, Host, BaseViewType.Home);
        UIButtonFactory.Navigate<BaseViewType>(replayButton, Host, BaseViewType.GamePlay);
    }

    // ==================================================
    // Inherited Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        if (data is not FinalScoreData scoreData)
        {
            Debug.Log($"data passed to game over view is not final score data, type: {data?.GetType().Name}");
            return;
        }

        gameScoreText.text = $"{scoreData.score}";
        highScoreText.text = $"{scoreData.highScore}";
    }
}
