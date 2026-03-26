using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    // i want to toggle a game over for testing purposes
    public bool InitiateGameOver;

    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Player playerPrefab;

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

    private DataManager data => DataManager.Instance;
    private UIManager UI => UIManager.Instance;

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
    }

    void Awake()
    {
        state = GameState.Fresh;
        picker = new PlayerPicker();
    }

    void OnEnable()
    {
        // Board events
        EventBus.Unsubscribe<GameOverEvent>(HandleGameOver);
        EventBus.Subscribe<TurnCompletedEvent>(HandleTurnCompleted);
        EventBus.Subscribe<WinEvent>(HandleWin);

        // Game state events
        EventBus.Subscribe<GameReadyEvent>(OnStartGame);
        EventBus.Subscribe<PauseGameEvent>(OnPauseGame);
        EventBus.Subscribe<ResumeGameEvent>(OnResumeGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);
    }

    void OnDisable()
    {
        // Board events
        EventBus.Unsubscribe<GameOverEvent>(HandleGameOver);
        EventBus.Unsubscribe<TurnCompletedEvent>(HandleTurnCompleted);
        EventBus.Unsubscribe<WinEvent>(HandleWin);

        // UI events
        EventBus.Unsubscribe<GameReadyEvent>(OnStartGame);
        EventBus.Unsubscribe<PauseGameEvent>(OnPauseGame);
        EventBus.Unsubscribe<ResumeGameEvent>(OnResumeGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
    }

    void Update()
    {
        if (InitiateGameOver)
        {
            RemoveCurrentPlayer();
            HandleGameOver(new GameOverEvent());    // for testing only!!!
        }
            
    }


    // ================================
    // UI Event Handlers
    // ================================

    private void OnStartGame(GameReadyEvent e)
    {
        Debug.Assert(state == GameState.Fresh, $"Start called with invalid state: {state}");

        InitializePlayerBoundaries(e.BoardBounds);

        state = GameState.Playing;
        SpawnNewPlayer();
    }

    private void OnPauseGame(PauseGameEvent e)
    {
        Debug.Assert(state == GameState.Playing, $"Pause called with invalid state: {state}");

        state = GameState.Paused;
        player.enabled = false;
    }

    private void OnResumeGame(ResumeGameEvent e)
    {
        Debug.Assert(state == GameState.Paused, $"Resume called with invalid state: {state}");

        state = GameState.Playing;
        player.enabled = true;
    }

    private void OnExitGame(ExitGameEvent e) // should only be called on user exit
    {
        Debug.Assert(state == GameState.Paused, $"Exit called with invalid state: {state}");

        state = GameState.Over;

        RemoveCurrentPlayer();
        EndGame();
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
        UI.UpdateScore(score);
    }


    private void HandleGameOver(GameOverEvent e)    // called on a game over
    {
        Debug.Assert(state == GameState.Playing, $"Initiate game over from invalid state: {state}");

        state = GameState.Over;
        EndGame();

        UI.ShowGameOver();
    }


    // ================================
    // Private Methods
    // ================================

    private void EndGame()
    {
        // good place to put data preparation

        // Reset the game
        picker.Reset();
        score = 0;
        state = GameState.Fresh;
    }

    private void InitializePlayerBoundaries(Bounds boardBounds)
    {
        if (playerBoundaries.size != Vector3.zero) return;  // player bounds already initialized

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

        UI.UpdatePlayerPreview(playerColors.nextColor);

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
