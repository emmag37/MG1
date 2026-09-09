using UnityEngine;
using UnityEngine.UI;


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
        Debug.Assert(settingsButton != null, "[GameOverView] Null settings button");
        Debug.Assert(homeButton != null, "[GameOverView] Null home button");
        Debug.Assert(replayButton != null, "[GameOverView] Null replay button");

        Debug.Assert(gameScoreText != null, "[GameOverView] Null game score text");
        Debug.Assert(highScoreText != null, "[GameOverView] Null high score text");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        UIButtonFactory.Navigate<PopUpViewType>(settingsButton, Host, PopUpViewType.Settings);
        UIButtonFactory.Navigate<BaseViewType>(homeButton, Host, BaseViewType.Home);
        UIButtonFactory.Navigate<BaseViewType>(replayButton, Host, BaseViewType.GamePlay);
    }

    protected override void SetInfo(IUIData data)
    {
        if (data is not FinalScoreData scoreData)
        {
            Debug.LogError($"[GameOverView] Data passed to set info is not final score data, type: {data?.GetType().Name}");
            return;
        }

        gameScoreText.text = $"{scoreData.score}";
        highScoreText.text = $"{scoreData.highScore}";
    }
}
