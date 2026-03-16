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

    [SerializeField] private SpriteView nextPlayerImage;

    [SerializeField] private Board board;

    // ================================
    // Private Fields
    // ================================

    private PlayerPicker picker;
    private Player player;
    
    private bool gameOver = false;
    private int score;
    
    private Bounds playerBoundaries;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        Debug.Assert(spawnPoint != null);
        Debug.Assert(playerPrefab != null);
        Debug.Assert(nextPlayerImage != null);
        Debug.Assert(board != null);

        picker = new PlayerPicker();

        InitPlayerBoundaries();

        board.FullBoard += HandleFullBoard;
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
        Debug.Assert(player != null);

        player.enabled = false;
    }

    /// <summary>
	/// Resumes the gameplay.
	/// </summary>
    public void Resume()
    {
        Debug.Assert(player != null);

        player.enabled = true;
    }


    // ================================
    // Event Handlers
    // ================================

    // runs the turn initiated by the player being released
    private void HandlePlayerReleasedOnBoard(Player playerReleased)
    {
        Debug.Assert(playerReleased && player && playerReleased == player, "Player released is not current player");

        Vector3 newPosition;
        bool validPosition = board.TryGetPlayerPosition(player.Position, out newPosition);

        if (!validPosition)                        
        {
            player.Move(spawnPoint.position);
            return;
        }

        player.Move(newPosition);                                    // render the snapping movement to the board

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

    private void HandleFullBoard()
    {
        gameOver = true;
    }


    // ================================
    // Private Methods
    // ================================

    private void RestartGame()
    {
        Debug.assert(player == null);

        gameOver = false;
        board.Reset();
        picker.Reset();
        score = 0;

        SpawnNewPlayer();
    }

    private void SpawnNewPlayer()
    {
        Debug.assert(player == null);

        var playerColors = picker.CalculateNewPlayerColors();
        nextPlayerImage.SetSprite(SpriteDatabase.Instance.sprites[playerColors.nextColor]);        

        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        player.Initialize(playerColors.color, SpriteDatabase.Instance.sprites[playerColors.color], playerBoundaries);      

        player.PlayerReleased += HandlePlayerReleasedOnBoard;
    }

    private void RemoveCurrentPlayer()
    {
        Debug.assert(player != null);

        player.PlayerReleased -= HandlePlayerReleasedOnBoard;        
        Destroy(player.gameObject);
    }

    private void InitPlayerBoundaries()
    {
        playerBoundaries = board.BoardBounds;
        
        Vector3 min = playerBoundaries.min;
        min.y = spawnPoint.position.y;

        playerBoundaries.SetMinMax(min, playerBoundaries.max);
    }

}
