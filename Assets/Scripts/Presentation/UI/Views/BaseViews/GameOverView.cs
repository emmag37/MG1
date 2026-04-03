using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game over screen.
/// </summary>
public class GameOverView : BaseView<IGameData>
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button homeButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button scoreHistoryButton;

    [SerializeField] private Text gameScoreText;
    [SerializeField] private Text highScoreText;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    void OnValidate()
    {
        Debug.Assert(homeButton != null, "Home button not set in game over view");
        Debug.Assert(replayButton != null, "Replay button not set in game over view");
        Debug.Assert(scoreHistoryButton != null, "Score history button not set in game over view");

        Debug.Assert(gameScoreText != null, "Game score text not set in game over view");
        Debug.Assert(highScoreText != null, "High score text not set in game over view");
    }

    void Awake()
    {
        homeButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.Home));
        replayButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.GamePlay));

        scoreHistoryButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.ScoreHistory));
    }

    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Show(IGameData data)
    {
        gameScoreText.text = $"{data.Score}";
        highScoreText.text = $"{data.HighScore}";

        base.Show(data);
    }
}
