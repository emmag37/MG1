using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public int Score => score;

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

    // ================================
    // Private Fields
    // ================================
    private UIManager Manager => UIManager.Instance;
    private GameDataService gameData;

    private int score;
    private int highScore;

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
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }

    // ================================
    // Public Methods
    // ================================

    public void Initialize(GameDataService gameData)
    {
        this.gameData = gameData;

        score = 0;
        highScore = gameData.GetGameData().HighScore;
        UpdateScoreText();
    }

    public void SetPlayerPreview(CellColor nextColor)
    {
        playerPreview.sprite = SpriteDatabase.Instance.GetSprite(nextColor);
    }

    public void AddPoints(int points)
    {
        if (points == 0) return;

        score += points;
        if (score > highScore)
        {
            highScore = points;
            gameData.UpdateHighScore(highScore);
        }

        UpdateScoreText();
    }

    public void Reset()
    {
        score = 0;
    }


    // ================================
    // Event Handlers
    // ================================

    // fix
    private void OnStartGame(StartGameEvent e)
    {
        Debug.Log("start game in HUD");

        //UpdateScore(e.Data.Score, e.Data.HighScore);

        HUDPanel.gameObject.SetActive(true);
    }

    private void OnGameOver(GameOverEvent e)
    {
        gameData.SetFinalScore(score);
        HUDPanel.gameObject.SetActive(false);
    }


    // ================================
    // Private Methods
    // ================================

    private void UpdateScoreText()
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }
}
