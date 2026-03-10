using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Controls the game states and initiates plays.
/// Bridges communication between player instances, the board, and UI updates.
/// </summary>
public class GamePlay : MonoBehaviour
{
    // ================================
    // Constants
    // ================================

    private const int RowSize = 5;  // if you change row size in the future it must be odd for an origin cell

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

    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Player playerPrefab;

    [SerializeField] private GamePieceImage nextPlayerImage;

    [SerializeField] private Board board;

    // ================================
    // Private Fields
    // ================================

    private PlayerPicker picker;
    private Player player;
    
    private bool gameOver = false;
    private int score;                      


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        picker = new PlayerPicker();

        board.BoardFull += HandleBoardFull;
    }

    void OnEnable()
    {
        if (gameOver) RestartGame();        // reset on subsequent games only
    }

    void OnDisable()
    {
        if (player == null) return;

        RemoveCurrentPlayer();
        gameOver = true;
    }

    void Start()
    {
        SpawnNewPlayer();
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Pauses the gameplay.
	/// </summary>
    public void Pause()
    {
        player.enabled = false;
    }

    /// <summary>
	/// Resumes the gameplay.
	/// </summary>
    public void Resume()
    {
        player.enabled = true;
    }


    // ================================
    // Event Handlers
    // ================================

    // runs the turn initiated by the player being released
    private void HandlePlayerReleasedOnBoard(Player playerReleased)
    {
        // check to make sure it's the same player?

        Vector3 newPosition;
        bool validPosition = board.TryGetPlayerPosition(player.Position, out newPosition);

        if (!validPosition)                        
        {
            player.ReturnToStart();
            return;
        }

        player.SnapToBoard(newPosition);                                    // render the snapping movement to the board

        int pointsScored = board.RunPlay(newPosition, player.Color);        // runs all board logic
        if (pointsScored > 0)
        {
            score += pointsScored;
            UpdateScore?.Invoke(score);
        }

        RemoveCurrentPlayer();

        if (gameOver)
        {
            GameOver?.Invoke(score);
            return;                                                         // don't respawn
        }

        SpawnNewPlayer();
    }

    private void HandleBoardFull()
    {
        gameOver = true;
    }


    // ================================
    // Private Methods
    // ================================

    private void RestartGame()
    {
        gameOver = false;
        board.Reset();
        picker.Reset();
        score = 0;

        SpawnNewPlayer();
    }

    private void SpawnNewPlayer()
    {
        var playerColors = picker.CalculateNewPlayerColors();
        nextPlayerImage.SetSprite(playerColors.nextColor);

        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        player.Initialize(playerColors.color, board.BoardBounds);

        player.PlayerReleasedOnBoard += HandlePlayerReleasedOnBoard;
    }

    private void RemoveCurrentPlayer()
    {
        player.PlayerReleasedOnBoard -= HandlePlayerReleasedOnBoard;        
        Destroy(player.gameObject);
    }
}
