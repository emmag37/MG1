using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================

    // to remove this reference, bind the action in your composition root
    [SerializeField] private GameManager gameManager;   // to remove

    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Image playerPreview;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(scoreText != null, "Score text not set");
        Debug.Assert(highScoreText != null, "High score text not set");

        Debug.Assert(playerPreview != null, "Player preview not set");
    }

    void OnEnable()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);

        EventBus.Subscribe<ScoreUpdateEvent>(OnScoreUpdate);
        EventBus.Subscribe<SpawnPlayerEvent>(OnPlayerPreviewUpdate);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        EventBus.Subscribe<ScoreUpdateEvent>(OnScoreUpdate);
        EventBus.Subscribe<SpawnPlayerEvent>(OnPlayerPreviewUpdate);
    }


    // ================================
    // Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        UpdateScore(0, e.HighScore);

        // show all elements
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void OnExitGame(ExitGameEvent e)
    {
        HideElements();
    }

    private void OnGameOver(GameOverEvent e)
    {
        HideElements();
    }

    public void OnScoreUpdate(ScoreUpdateEvent e)
    {
        UpdateScore(e.Score, e.HighScore);
    }

    public void OnPlayerPreviewUpdate(SpawnPlayerEvent e)
    {
        playerPreview.sprite = SpriteDatabase.Instance.GetSprite(e.NextColor);
    }


    // ================================
    // Private Methods
    // ================================

    private void HideElements()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void UpdateScore(int score, int highScore)
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }
}
