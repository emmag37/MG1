using UnityEngine;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private BoardController board;

    [SerializeField] private TutorialController tutorial;

    // ================================
    // Private Types
    // ================================
    private enum GameState
    {
        Playing,
        Paused,
        Over,
        Fresh,
        Tutorial
    }

    // ================================
    // Private Fields
    // ================================
    private GameDataService dataService;
    private PlayerPicker picker;

    private GameState state;
    private bool activePlayer;

    private int score;
    private int highScore;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnEnable()
    {
        // Board events
        EventBus.Subscribe<WinEvent>(HandleWin);
    }

    void OnDisable()
    {
        // Board events
        EventBus.Unsubscribe<WinEvent>(HandleWin);
    }

    void OnDestroy()
    {
        board.FullBoard -= HandleFullBoard;
        board.TurnCompleted -= HandleTurnCompleted;
    }


    // ================================
    // Initialize
    // ================================

    public void Initialize(GameDataService dataService)
    {
        this.dataService = dataService;

        picker = new PlayerPicker();
        board.Initialize();

        tutorial.Initialize(board);

        state = GameState.Fresh;
        activePlayer = false;

        score = 0;
        highScore = dataService.GetGameData().HighScore;

        board.FullBoard += HandleFullBoard;
        board.TurnCompleted += HandleTurnCompleted;
    }


    // ================================
    // UI Commands
    // ================================

    public void StartGame()
    {
        if (state == GameState.Tutorial)
            state = GameState.Fresh;

        if (state != GameState.Fresh)
            ResetGame();
        Debug.Assert(state == GameState.Fresh, $"Game not reset, still in: {state}");

        EventBus.Publish(new StartGameEvent { Data = dataService.GetGameData() });   // prepare systems not owned by the game manager

        state = GameState.Playing;
        SpawnNewPlayer();
    }

    public void ExitGame()  // User exit!
    {
        Debug.Assert(state == GameState.Paused, $"Exit called with invalid state: {state}");

        state = GameState.Over;

        Debug.Log("all data will be lost");

        EventBus.Publish(new ExitGameEvent());
    }

    public void PauseGame()
    {
        Debug.Assert(state == GameState.Playing, $"Pause called with invalid state: {state}");

        state = GameState.Paused;
        EventBus.Publish(new PauseGameEvent());
    }

    public void ResumeGame()
    {
        Debug.Assert(state == GameState.Paused, $"Resume called with invalid state: {state}");

        state = GameState.Playing;
        EventBus.Publish(new ResumeGameEvent());
    }

    public void RunTutorial()
    {
        state = GameState.Tutorial; // turn off active player checking

        tutorial.StartTutorial();
    }

    public void SkipTutorial()
    {
        tutorial.SkipTutorial();
    }


    // ================================
    // Board Event Handlers
    // ================================

    // moves the player back to start or on the board.
    // if on the board, executes the player's turn.
    private void HandleTurnCompleted(int points)  // turn this into a local event
    {
        if (state == GameState.Tutorial) return;

        Debug.Assert(state == GameState.Playing, $"Turn ran during invalid state: {state}");
        Debug.Assert(activePlayer, "Player turn completed but no active player");

        RemoveCurrentPlayer();

        if (state == GameState.Playing) SpawnNewPlayer();
    }

    private void HandleFullBoard()    // called on a game over  - turn this into a local event
    {
        Debug.Assert(state == GameState.Playing, $"Initiate game over from invalid state: {state}");

        state = GameState.Over;

        if (score == highScore)
            dataService.SetHighScore(highScore);
        dataService.AddScore(score);

        EventBus.Publish(new GameOverEvent { Data = dataService.GetGameData() });
    }

    // move this logic to turn completed, the local event should pass a bool
    // the score update should then be the win
    private void HandleWin(WinEvent e)
    {
        if (e.Points == 0) return;

        score += e.Points;
        if (score > highScore)
        {
            highScore = score;
        }

        EventBus.Publish(new ScoreUpdateEvent { Score = score, HighScore = highScore });
    }

    // ================================
    // Private Methods
    // ================================

    private void ResetGame()
    {
        RemoveCurrentPlayer();

        board.Reset();
        picker.Reset();

        score = 0;
        highScore = dataService.GetGameData().HighScore;

        state = GameState.Fresh;
    }

    private void SpawnNewPlayer()
    {
        Debug.Assert(!activePlayer, "Tried to spawn while player active");

        if (state == GameState.Over) return;      // don't respawn on game over
        
        var playerColors = picker.CalculateNewPlayerColors();
        EventBus.Publish(new SpawnPlayerEvent { Color = playerColors.Color, NextColor = playerColors.NextColor });

        activePlayer = true;
    }

    private void RemoveCurrentPlayer()
    {
        Debug.Assert(activePlayer, "Tried to remove when no active player");

        EventBus.Publish(new DestroyPlayerEvent());

        activePlayer = false;
    }
}
