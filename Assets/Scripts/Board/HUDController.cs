using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public int Score => score;

    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private Image HUDPanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Image playerPreview;


    // ================================
    // Private Fields
    // ================================
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

    // ================================
    // Public Methods
    // ================================

    public void Initialize(bool loadGame, GameDataService gameData)
    {
        this.gameData = gameData;

        // load score
        score = gameData.GetGamePlayData().CurrentScore;    // reliably resets, should always be accurate
        highScore = gameData.GetGameData().HighScore;
        UpdateScoreText();

        // load preview data
        if (loadGame)
        {
            CellColor preview = gameData.GetGamePlayData().NextPlayer;
            SetPlayerPreview(preview);
        }
    }

    public (int, int) GameOver()
    {
        gameData.SetFinalScore(score);

        return (score, highScore);
    }

    public void Reset()
    {
        score = 0;
        UpdateScoreText();
    }

    public void SetPlayerPreview(CellColor nextColor)
    {
        Debug.Log($"Preview color: {nextColor}");
        playerPreview.sprite = SpriteDatabase.Instance.GetSprite(nextColor);
    }

    public int AddPoints(int points)
    {
        if (points > 0)
        {
            score += points;
            if (score > highScore)
            {
                highScore = points;
                gameData.UpdateHighScore(highScore);
            }

            UpdateScoreText();
        }
        return score;
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
