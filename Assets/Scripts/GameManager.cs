using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Game State
    private enum GameState { Active, Inactive };   
    private static GameState state;

    // private variables
    [SerializeField] private GamePlay gamePlay;
    private int highScore;

    // Canvases
    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameSettingsCanvas;

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

        // load high score
        highScore = PlayerPrefs.GetInt("highScore", 0);

        // initialize game state
        state = GameState.Inactive;
        Debug.Log("Inactive");

    }

    // event handlers
    private void HandleGameOver(int score)
    {
        // update the game over text
        gameOverScoreText.text = $"{score}";

        // exit the game scene
        EndGame(gameOverCanvas);
    }

    private void HandleNewScore(int score)
    {
        UpdateScoreText(score);
    }

    // button functions
    // only called from home screen
    public void OnPlayButtonClicked()
    {
        // navigate to new game from home
        NewGame(homeCanvas);
    }

    // called from game settings and game over
    public void OnReplayButtonClicked(string button)
    {
        // choose which canvas called the func
        GameObject canvas = ChooseCanvas(button);

        // create a new game
        NewGame(canvas);
    }

    // called from game settings and game over
    public void OnHomeButtonClicked(string button)
    {
        // choose which canvas called the function
        GameObject canvas = ChooseCanvas(button);

        // ends game if one is active and enables the home screen
        EndGame(homeCanvas);

        // disable the current canvas
        canvas.SetActive(false);
    }

    public void OnSettingsClicked()
    {
        // enable settings canvas on top of game
        gameSettingsCanvas.SetActive(true);

        // pause the game play
        gamePlay.Pause();
    }

    public void OnSettingsExitClicked()
    {
        // remove settings canvas
        gameSettingsCanvas.SetActive(false);

        // resume game play
        gamePlay.Resume();
    }

    // helper functions
    // navigates to a new game from the given canvas
        // old game must be closed before hand
        // only function that changes the game state to active
    private void NewGame(GameObject currentCanvas)
    {
        // makes sure to close any currently running game
        if (state == GameState.Active) EndGame(currentCanvas);

        // enable new game
        UpdateScoreText(0);
        gamePlay.gameObject.SetActive(true);        // game play resets itself on enable
        gameCanvas.SetActive(true);

        // disable the current canvas
        currentCanvas.SetActive(false);

        state = GameState.Active;
        Debug.Log("Active");
    }

    // navigates out of a game to the given canvas
        // changes the game state to inactive
    private void EndGame(GameObject nextCanvas)
    {
        // exit game
        gamePlay.gameObject.SetActive(false);
        gameCanvas.SetActive(false);

        // enable new canvas
        nextCanvas.SetActive(true);

        state = GameState.Inactive;
        Debug.Log("Inactive");
    }

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

    private GameObject ChooseCanvas(string name)
    {
        GameObject canvas = null;
        if (name == "gameOver")
        {
            canvas = gameOverCanvas;
        }
        else if (name == "gameSettings")
        {
            canvas = gameSettingsCanvas;
        }
        return canvas;
    }
}
