using UnityEngine;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ================================
    // Events
    // ================================

    public Action<int, int> UpdateScore;
    public Action<CellColor> UpdatePlayerPreview;

    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private BoardController board;

    // ================================
    // Private Types
    // ================================

    private enum GameState
    {
        Playing,
        Paused,
        Over,
        Fresh
    }

    // ================================
    // Private Fields
    // ================================

    private GameState state;
    private bool activePlayer;

    private int score;

    private PlayerPicker picker;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        state = GameState.Fresh;
        activePlayer = false;

        picker = new PlayerPicker();
    }

    void OnEnable()
    {
        // Board events
        board.FullBoard += HandleFullBoard;
        board.TurnCompleted += HandleTurnCompleted;
        EventBus.Subscribe<WinEvent>(HandleWin);
    }

    void OnDisable()
    {
        // Board events
        board.FullBoard -= HandleFullBoard;
        board.TurnCompleted -= HandleTurnCompleted;
        EventBus.Unsubscribe<WinEvent>(HandleWin);
    }


    // ================================
    // UI Commands
    // ================================

    public void StartGame()
    {
        if (state != GameState.Fresh)
        {
            PrepareGame();      // prepare fields owned by game manager
        }
        Debug.Assert(state == GameState.Fresh, $"Game not reset, still in: {state}");

        // update high score with new data system
        EventBus.Publish(new StartGameEvent { HighScore = 0 });   // prepare systems not owned by the game manager

        state = GameState.Playing;
        SpawnNewPlayer();
    }

    public void ExitGame()  // User exit!
    {
        Debug.Assert(state == GameState.Paused, $"Exit called with invalid state: {state}");

        state = GameState.Over;

        // save any necessary data

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


    // ================================
    // Board Event Handlers
    // ================================

    // moves the player back to start or on the board.
    // if on the board, executes the player's turn.
    private void HandleTurnCompleted()  // turn this into a local event
    {
        Debug.Assert(state == GameState.Playing, $"Turn ran during invalid state: {state}");
        Debug.Assert(activePlayer, "Player turn completed but no active player");

        RemoveCurrentPlayer();

        if (state == GameState.Playing) SpawnNewPlayer();
    }

    private void HandleFullBoard()    // called on a game over  - turn this into a local event
    {
        Debug.Assert(state == GameState.Playing, $"Initiate game over from invalid state: {state}");

        state = GameState.Over;

        // handle any data saves

        // update high score with new data system
        
        EventBus.Publish(new GameOverEvent { ScoreData = new UserScore { Score = score, HighScore = 0 } });
    }

    private void HandleWin(WinEvent e)
    {
        score += e.Points;

        // set score in data
        UpdateScore?.Invoke(score, 0);  // update high score with new data system
    }

    // ================================
    // Private Methods
    // ================================

    private void PrepareGame()
    {
        // good place to put data preparation

        RemoveCurrentPlayer();

        board.Reset();
        picker.Reset();
        score = 0;
        state = GameState.Fresh;
    }

    private void SpawnNewPlayer()
    {
        Debug.Assert(!activePlayer, "Tried to spawn while player active");

        if (state == GameState.Over) return;      // don't respawn on game over

        var playerColors = picker.CalculateNewPlayerColors();

        UpdatePlayerPreview?.Invoke(playerColors.NextColor);
        EventBus.Publish(new SpawnPlayerEvent { Color = playerColors.Color });

        activePlayer = true;
    }

    private void RemoveCurrentPlayer()
    {
        Debug.Assert(activePlayer, "Tried to remove when no active player");

        EventBus.Publish(new DestroyPlayerEvent());

        activePlayer = false;
    }
}
