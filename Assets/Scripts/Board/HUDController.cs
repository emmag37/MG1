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


    // ================================
    // Public Methods
    // ================================

    public void Initialize(GameDataService gameData)
    {
        this.gameData = gameData;

        score = gameData.GetGamePlayData().CurrentScore;    // reliably resets, should always be accurate
        highScore = gameData.GetGameData().HighScore;

        UpdateScoreText();
    }

    public void GameOver()
    {
        gameData.SetFinalScore(score);
    }

    public void Reset()
    {
        score = 0;
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


    // ================================
    // Private Methods
    // ================================

    private void UpdateScoreText()
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }
}
