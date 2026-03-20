using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game over screen.
/// </summary>
public class GameOverView: BaseView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button homeButton;
    [SerializeField] private Button replayButton;

    [SerializeField] private Text gameOverScoreText;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(homeButton != null, "Home button not set in game over view");
        Debug.Assert(replayButton != null, "Replay button not set in game over view");
        Debug.Assert(gameOverScoreText != null, "Game over score text not set in game over view");
    }

    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Show(ViewData data)
    {
        int score = data.FinalScore;
        gameOverScoreText.text = $"{score}";

        base.Show();
    }

    // ==================================================
    // Public Methods
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
