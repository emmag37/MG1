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

    private int score;
    private int highScore;

    private ISpriteDatabase spriteDatabase;

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

    public void Initialize(int highScore, int score = 0, CellColor previewColor = CellColor.Empty)
    {
        this.highScore = highScore;

        spriteDatabase = ServiceLocator.Get<ISpriteDatabase>();
        UpdateScoreText();
    }

    public void LoadGame(int score, CellColor previewColor)
    {
        this.score = score;

        if (score > highScore)
            highScore = score;

        UpdateScoreText();
        SetPlayerPreview(previewColor);
    }

    public (int, int) GameOver()
    {
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
        playerPreview.sprite = spriteDatabase.GetSprite((int)nextColor);
    }

    public int AddPoints(int points)
    {
        if (points > 0)
        {
            score += points;

            if (score > highScore)
                highScore = score;

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
