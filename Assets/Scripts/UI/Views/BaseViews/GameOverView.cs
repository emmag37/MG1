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
        Debug.Assert(homeButton != null, "Home button not set in game over view");
        Debug.Assert(replayButton != null, "Replay button not set in game over view");
        Debug.Assert(settingsButton != null, "Settings button not set in game over view");

        Debug.Assert(gameScoreText != null, "Game score text not set in game over view");
        Debug.Assert(highScoreText != null, "High score text not set in game over view");
    }

    void Awake()
    {
        homeButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.Home));
        replayButton.onClick.AddListener(() => Manager.ShowView(BaseViewType.GamePlay));

        settingsButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.Settings));
    }

    // ==================================================
    // Inherited Methods
    // ==================================================

    protected override void SetInfo(IRuntimeData data)
    {
        if (data is not IGameData gameData)
        {
            Debug.Log($"data passed to game over view is not game data, type: {data?.GetType().Name}");
            return;
        }

        gameScoreText.text = $"{gameData.Score}";
        highScoreText.text = $"{gameData.HighScore}";
    }
}
