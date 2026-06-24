using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================

    // to remove this reference, bind the action in your composition root
    [SerializeField] private GameManager gameManager;   // to remove

    [SerializeField] private Image HUDPanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Image playerPreview;

    [SerializeField] private Button pauseButton;

    // private fields
    private UIManager Manager => UIManager.Instance;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(scoreText != null, "Score text not set");
        Debug.Assert(highScoreText != null, "High score text not set");

        Debug.Assert(playerPreview != null, "Player preview not set");
    }

    void Awake()
    {
        pauseButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.Pause));
    }

    void OnEnable()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);

        EventBus.Subscribe<SpawnPlayerEvent>(OnPlayerPreviewUpdate);
        EventBus.Subscribe<ScoreUpdateEvent>(OnScoreUpdate);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        EventBus.Subscribe<SpawnPlayerEvent>(OnPlayerPreviewUpdate);
        EventBus.Unsubscribe<ScoreUpdateEvent>(OnScoreUpdate);
    }


    // ================================
    // Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        Debug.Log("start game in HUD");

        UpdateScore(e.Data.Score, e.Data.HighScore);

        HUDPanel.gameObject.SetActive(true);
    }

    private void OnExitGame(ExitGameEvent e)
    {
        HUDPanel.gameObject.SetActive(false);
    }

    private void OnGameOver(GameOverEvent e)
    {
        HUDPanel.gameObject.SetActive(false);
    }

    public void OnPlayerPreviewUpdate(SpawnPlayerEvent e)
    {
        playerPreview.sprite = SpriteDatabase.Instance.GetSprite(e.NextColor);
    }

    public void OnScoreUpdate(ScoreUpdateEvent e)
    {
        UpdateScore(e.Score, e.HighScore);
    }


    // ================================
    // Private Methods
    // ================================

    private void UpdateScore(int score, int highScore)
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }
}
