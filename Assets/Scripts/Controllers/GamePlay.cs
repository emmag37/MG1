using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GamePlay : MonoBehaviour
{

    // i want to toggle a game over for testing purposes
    public bool InitiateGameOver;


    // ================================
    // Events
    // ================================

    /// <summary>
	/// Invoked when there is a game over.
	/// </summary>
    public event Action<int> GameOver;

    /// <summary>
	/// Invoked when the player scores points.
	/// </summary>
    public event Action<int> UpdateScore;

    /// <summary>
	/// Invoked when a new player preview color is set.
	/// </summary>
    public event Action<CellColor> UpdatePlayerPreview;

    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Board board;

    // ================================
    // Private Types
    // ================================

    private enum GameState
    {
        Playing,
        Paused,
        GameOver,
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
        Debug.Assert(board != null, "Board not set");
    }

    void Awake()
    {
        state = GameState.Fresh;
        picker = new PlayerPicker();

        board.FullBoard += HandleFullBoard;
    }

    void Update()
    {
        if (InitiateGameOver)
        {
            RemoveCurrentPlayer();
            HandleFullBoard();    // for testing only!!!
        }
            
    }

    void OnDestroy()
    {
        board.FullBoard -= HandleFullBoard;
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Initializes the game values that are dependent on other assests
	/// being ready.
	/// </summary>
    public void Initialize()
    {
        InitializePlayerBoundaries();
    }

    /// <summary>
	/// Starts new game.
	/// </summary>
    public void StartGame()
    {
        Debug.Assert(state == GameState.Fresh, $"Start called with invalid state: {state}");

        state = GameState.Playing;

        SpawnNewPlayer();
    }

    /// <summary>
	/// Pauses the gameplay.
	/// </summary>
    public void PauseGame()
    {
        Debug.Assert(state == GameState.Playing, $"Pause called with invalid state: {state}");

        state = GameState.Paused;
        player.enabled = false;
    }

    /// <summary>
	/// Resumes the gameplay.
	/// </summary>
    public void ResumeGame()
    {
        Debug.Assert(state == GameState.Paused, $"Resume called with invalid state: {state}");

        state = GameState.Playing;
        player.enabled = true;
    }

    /// <summary>
	/// Ends the game play and erases the scene.
	/// </summary>
    public void ExitGame()
    {
        Debug.Assert(state == GameState.Playing || state == GameState.GameOver, $"Exit called with invalid state: {state}");

        if (state == GameState.Playing)      // user exit
            RemoveCurrentPlayer();

        board.Reset();
        picker.Reset();
        score = 0;

        state = GameState.Fresh;
    }


    // ================================
    // Event Handlers
    // ================================

    // moves the player back to start or on the board.
    // if on the board, executes the player's turn.
    private void HandlePlayerReleased(Player playerReleased)
    {
        Debug.Assert(state == GameState.Playing, $"Player released during invalid state: {state}");
        Debug.Assert(player != null, "Player released but no active player");
        Debug.Assert(player == playerReleased, "Player released is not the current player");

        Vector2Int index;
        if (!TryPlacePlayer(player.Position, out index)) return;

        ExecuteTurn(index, player.Color);
        RemoveCurrentPlayer();

        if (state == GameState.Playing) SpawnNewPlayer();
    }

    private void HandleFullBoard()
    {
        Debug.Assert(state == GameState.Playing, $"Initiate game over from invalid state: {state}");

        state = GameState.GameOver;
        GameOver?.Invoke(score);
    }


    // ================================
    // Private Methods
    // ================================

    private void InitializePlayerBoundaries()
    {
        playerBoundaries = board.BoardBounds;   // incorrect on first run

        Vector3 min = playerBoundaries.min;
        min.y = spawnPoint.position.y;

        playerBoundaries.SetMinMax(min, playerBoundaries.max);
    }

    private void SpawnNewPlayer()
    {
        Debug.Assert(player == null, "Player still in existence");  // player should be null on start?

        if (state == GameState.GameOver) return;      // don't respawn on game over

        var playerColors = picker.CalculateNewPlayerColors();

        UpdatePlayerPreview?.Invoke(playerColors.nextColor);

        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        player.Initialize(playerColors.color, playerBoundaries);

        player.PlayerReleased += HandlePlayerReleased;
    }

    private void RemoveCurrentPlayer()
    {
        Debug.Assert(player != null, "No actve player");

        player.PlayerReleased -= HandlePlayerReleased;        
        Destroy(player.gameObject);
        player = null;
    }

    private bool TryPlacePlayer(Vector3 position, out Vector2Int index)
    {
        Vector3 newPosition;
        bool valid = board.TryGetPlayerPosition(position, out newPosition, out index);

        if (!valid)
        {
            player.Move(spawnPoint.position);
        } else
        {
            player.Move(newPosition);                                    // render the snapping movement to the board
        }

        return valid;
    }

    private void ExecuteTurn(Vector2Int index, CellColor color)
    {
        int pointsScored = board.RunPlay(index, color);

        if (pointsScored > 0)
        {
            score += pointsScored;
            UpdateScore?.Invoke(score);
        }
    }
}
