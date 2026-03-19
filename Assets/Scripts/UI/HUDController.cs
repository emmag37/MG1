using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================

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


    // ================================
    // Public Methods
    // ================================

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateScoreText(int score, int highScore)
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }

    public void UpdatePlayerPreviewSprite(CellColor color)
    {
        playerPreview.sprite = SpriteDatabase.Instance.GetSprite(color);
    }
}
