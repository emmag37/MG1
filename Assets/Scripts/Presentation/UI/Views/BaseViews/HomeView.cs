using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the home screen.
/// </summary>
public class HomeView: BaseView<PlayerProfile>
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

    void Awake()
    {
        profileButton.onClick.AddListener(() => UI.PushOverlay(PopUpViewType.Profile));
        scoreHistoryButton.onClick.AddListener(() => UI.PushOverlay(PopUpViewType.ScoreHistory));
        playButton.onClick.AddListener(UI.GameManager.StartGame);
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(PlayerProfile data)
    {
        profileButton.image.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        base.Show(data);
    }

    public override void UpdateView(PlayerProfile data)
    {
        profileButton.image.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        base.UpdateView(data);
    }
}
