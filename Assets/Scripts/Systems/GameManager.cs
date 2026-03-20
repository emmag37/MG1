using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GameManager : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private GamePlay gamePlay;
    [SerializeField] private UIManager uiManager;

    // ================================
    // Private Fields
    // ================================

    private int highScore;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(gamePlay != null, "Game play not set");
        Debug.Assert(uiManager != null, "UI manager not set");
    }

    void Start()
    {
        // subscribe to the game play events
        gamePlay.GameOver += HandleGameOver;
        gamePlay.UpdateScore += HandleNewScore;
        gamePlay.UpdatePlayerPreview += HandleNewPlayerPreview;

        // subscribe to ui events
        uiManager.StartGame += HandleStartGame;
        uiManager.EndGame += HandleEndGame;
        uiManager.PauseGame += HandlePauseGame;
        uiManager.ResumeGame += HandleResumeGame;

        // load high score
        highScore = PlayerPrefs.GetInt("highScore", 0);
        uiManager.UpdateScore(0, highScore);
    }

    void OnDestroy()
    {
        // unsubscribe from the game play events
        gamePlay.GameOver -= HandleGameOver;
        gamePlay.UpdateScore -= HandleNewScore;
        gamePlay.UpdatePlayerPreview -= HandleNewPlayerPreview;

        // unsubscribe from the ui events
        uiManager.StartGame -= HandleStartGame;
        uiManager.EndGame -= HandleEndGame;
        uiManager.PauseGame -= HandlePauseGame;
        uiManager.ResumeGame -= HandleResumeGame;
    }
    

    // ================================
    // Event Handlers
    // ================================

    // Game Play Events
    private void HandleGameOver(int score)
    {
        ViewData data = new ViewData(score);
        uiManager.ShowView(BaseViewType.GameOver, data);    // this will initiate end game
    }

    private void HandleNewScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
            PlayerPrefs.Save();
        }

        uiManager.UpdateScore(score, highScore);
    }

    private void HandleNewPlayerPreview(CellColor color)
    {
        uiManager.UpdatePlayerPreview(color);
    }

    // UI Manager Events
    private void HandleStartGame()
    {
        gamePlay.gameObject.SetActive(true);

        gamePlay.Initialize();
        gamePlay.StartGame();
    }

    private void HandleEndGame()
    {
        gamePlay.ExitGame();
        gamePlay.gameObject.SetActive(false);
    }

    private void HandlePauseGame()
    {
        gamePlay.PauseGame();
    }

    private void HandleResumeGame()
    {
        gamePlay.ResumeGame();
    }

}
