using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game over screen.
/// </summary>
public class GameOverView: UIView
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
    // Button Methods
    // ==================================================

    public void OnHomeClicked()
    {
        UIManager.Instance.LaunchHomeScreen(false);
    }

    public void OnReplayClicked()
    {
        UIManager.Instance.LaunchNewGame(false);
    }


    // ==================================================
    // Text Methods
    // ==================================================

    public void UpdateGameOverScoreText(int score)
    {
        gameOverScoreText.text = $"{score}";
    }

}
