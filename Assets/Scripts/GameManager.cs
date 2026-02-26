using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // private variables
    [SerializeField] private GamePlay gamePlay;

    // Canvases
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    // text
    [SerializeField] private Text gameOverScoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to the game play events
        gamePlay.GameOver += HandleGameOver;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void OnReplayButtonClicked()
    {
        Debug.Log("Replay Button");

        // enable the game play
        gamePlay.gameObject.SetActive(true);

        // switch to the game play canvas
        gameOverCanvas.SetActive(false);
        gameCanvas.SetActive(true);
    }
}
