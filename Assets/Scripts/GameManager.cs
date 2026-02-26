using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // private variables
    [SerializeField] private GamePlay gamePlay;
    private int highScore;

    // Canvases
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    // text
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text gameOverScoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to the game play events
        gamePlay.GameOver += HandleGameOver;
        gamePlay.UpdateScore += HandleNewScore;

        // load high score and initialize score text
        highScore = PlayerPrefs.GetInt("highScore", 0);
        UpdateScoreText(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // event handlers
    private void HandleGameOver(int score)
    {
        Debug.Log("Game Over");

        // update the game over text
        gameOverScoreText.text = $"{score}";

        // disable game play and hide the game container
        gamePlay.gameObject.SetActive(false);

        // switch to the game over canvas
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
    }

    private void HandleNewScore(int score)
    {
        UpdateScoreText(score);
    }

    // button functions
    public void OnReplayButtonClicked()
    {
        Debug.Log("Replay Button");

        // enable the game play and initialize the score
        gamePlay.gameObject.SetActive(true);
        UpdateScoreText(0);

        // switch to the game play canvas
        gameOverCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }

    // helper functions
    private void UpdateScoreText(int score)
    {
        scoreText.text = $"{score}";

        // update high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
            PlayerPrefs.Save();
        }

        highScoreText.text = $"{highScore}";
    }
}
