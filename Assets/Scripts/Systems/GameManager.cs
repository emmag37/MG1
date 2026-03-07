/**
 * Insert File Description
 * 
 */

using UnityEngine;
using UnityEngine.UI;

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
    //private enum GameState { Active, Inactive };    // update this
    //private static GameState state;

    private int highScore;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Start()
    {
        // subscribe to the game play events
        gamePlay.GameOver += HandleGameOver;
        gamePlay.UpdateScore += HandleNewScore;

        // subscribe to ui events
        uiManager.StartGame += HandleStartGame;
        uiManager.EndGame += HandleEndGame;
        uiManager.PauseGame += HandlePauseGame;
        uiManager.ResumeGame += HandleResumeGame;

        // load high score
        highScore = PlayerPrefs.GetInt("highScore", 0);

        uiManager.UpdateScoreText(0, highScore);
    }
    

    // ================================
    // Event Handlers
    // ================================

    // Game Play Events
    private void HandleGameOver(int score)
    {
        // initiate game over ui
        uiManager.GameOver(score);

        // exit the game scene
        EndGame();
    }

    private void HandleNewScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", highScore);
            PlayerPrefs.Save();
        }

        uiManager.UpdateScoreText(score, highScore);
    }

    // UI Manager Events
    private void HandleStartGame()
    {
        gamePlay.gameObject.SetActive(true);
    }

    private void HandleEndGame()
    {
        EndGame();
    }

    private void HandlePauseGame()
    {
        gamePlay.Pause();
    }

    private void HandleResumeGame()
    {
        gamePlay.Resume();
    }


    // ================================
    // Private Methods
    // ================================

    private void EndGame()
    {
        gamePlay.gameObject.SetActive(false);
    }

}
