using UnityEngine;
using UnityEngine.UI;

// note : consider switching to uint project wide

public class HUDController : MonoBehaviour
{
    // ================================
    // Public Fields
    // ================================
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
        Debug.Assert(HUDPanel != null, "[HUDController] HUD Panel not set");
        Debug.Assert(scoreText != null, "[HUDController] Score text not set");
        Debug.Assert(highScoreText != null, "[HUDController] High score text not set");
        Debug.Assert(playerPreview != null, "[HUDController] Player preview not set");
    }

    // ================================
    // Public Methods
    // ================================

    public void Initialize(int highScore)
    {
        this.highScore = Mathf.Max(0, highScore);   // clamp to 0 if bad data from save

        spriteDatabase = ServiceLocator.Get<ISpriteDatabase>();
        UpdateScoreText();
    }

    public void LoadGame(int score, CellColor previewColor)
    {
        this.score = Mathf.Max(0, score);   // clamp to 0 if bad data from save

        if (score > highScore)
            highScore = score;

        UpdateScoreText();
        SetPlayerPreview(previewColor);
    }

    public (int score, int highScore) GetScores()
    {
        return (score, highScore);  // could probably just return the record here
    }

    public void Reset()
    {
        score = 0;
        UpdateScoreText();
    }

    public void SetPlayerPreview(CellColor nextColor)
    {
        if (nextColor == CellColor.Empty)
        {
            Debug.LogWarning("[HUDController] Empty color passed to SetPlayerPreview");
            return;
        }

        if (spriteDatabase.TryGetSprite((int)nextColor, out Sprite previewSprite))
            playerPreview.sprite = previewSprite;
    }

    public int AddPoints(int points)
    {
        if (points <= 0)
        {
            Debug.LogWarning($"[HUDController] Non-positive value passed to AddPoints: {points}");
            return 0;
        }

        score += points;
        if (score > highScore)
            highScore = score;
        UpdateScoreText();

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
