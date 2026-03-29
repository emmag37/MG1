using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] private UIManager UI;  // need to take this out
    [SerializeField] private DataManager data;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private SpriteRenderer boardView;
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

    private PlayerPicker picker;
    private Player player;
    private Bounds playerBoundaries;

    private int score;
   

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(spawnPoint != null, "Spawn point not set");
        Debug.Assert(playerPrefab != null, "Player prefab not set");
        // add asserts
    }

    void Awake()
    {
        state = GameState.Fresh;
        picker = new PlayerPicker();

        InitializePlayerBoundaries(boardView.bounds);
    }

    void OnEnable()
    {
        // Board events
        EventBus.Subscribe<FullBoardEvent>(HandleFullBoard);
        EventBus.Subscribe<TurnCompletedEvent>(HandleTurnCompleted);
        EventBus.Subscribe<WinEvent>(HandleWin);
    }

    void OnDisable()
    {
        // Board events
        EventBus.Unsubscribe<FullBoardEvent>(HandleFullBoard);
        EventBus.Unsubscribe<TurnCompletedEvent>(HandleTurnCompleted);
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

        EventBus.Publish(new StartGameEvent());   // prepare systems not owned by the game manager

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
        player.enabled = false;

        EventBus.Publish(new PauseGameEvent());
    }

    public void ResumeGame()
    {
        Debug.Assert(state == GameState.Paused, $"Resume called with invalid state: {state}");

        state = GameState.Playing;
        player.enabled = true;

        EventBus.Publish(new ResumeGameEvent());
    }


    // ================================
    // Board Event Handlers
    // ================================

    // moves the player back to start or on the board.
    // if on the board, executes the player's turn.
    private void HandleTurnCompleted(TurnCompletedEvent e)
    {
        Debug.Assert(state == GameState.Playing, $"Turn ran during invalid state: {state}");
        Debug.Assert(player != null, "Player turn completed but no active player");

        RemoveCurrentPlayer();

        if (state == GameState.Playing) SpawnNewPlayer();
    }

    private void HandleWin(WinEvent e)
    {
        score += e.Points;

        data.SetScore(score);
        UI.UpdateScore(score);  // fix this
    }


    private void HandleFullBoard(FullBoardEvent e)    // called on a game over
    {
        Debug.Assert(state == GameState.Playing, $"Initiate game over from invalid state: {state}");

        state = GameState.Over;

        // handle any data saves

        EventBus.Publish(new GameOverEvent());
    }


    // ================================
    // Private Methods
    // ================================

    private void PrepareGame()
    {
        // good place to put data preparation

        // Reset the game
        RemoveCurrentPlayer();
        picker.Reset();
        score = 0;
        state = GameState.Fresh;

        // Send an event to prepare board, ui, sound?
        // ui already knows to start the event
    }

    private void InitializePlayerBoundaries(Bounds boardBounds)
    {
        playerBoundaries = boardBounds;

        Vector3 min = playerBoundaries.min;
        min.y = spawnPoint.position.y;

        playerBoundaries.SetMinMax(min, playerBoundaries.max);
    }

    private void SpawnNewPlayer()
    {
        Debug.Assert(player == null, "Player still in existence");  // player should be null on start?

        if (state == GameState.Over) return;      // don't respawn on game over

        var playerColors = picker.CalculateNewPlayerColors();

        UI.UpdatePlayerPreview(playerColors.nextColor); // fix this

        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        player.Initialize(playerColors.color, playerBoundaries);
    }

    private void RemoveCurrentPlayer()
    {
        Debug.Assert(player != null, "No actve player");
        
        Destroy(player.gameObject);
        player = null;
    }
}
