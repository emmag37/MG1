using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 
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

    public event Action<int> GameOver;
    public event Action<int> UpdateScore;

    // ================================
    // Inspector Fields
    // ================================

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GamePieceImage nextPlayerImage;

    public Player playerPrefab;

    // ================================
    // Private Fields
    // ================================

    private Board board;
    private PlayerPicker picker;

    private Player player;                  // current active player
    private bool gameOver = false;
    private int score;                      // current game score


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        board = transform.GetChild(0).GetComponent<Board>();
        picker = new PlayerPicker();

        board.BoardFull += HandleBoardFull;
    }

    // use to restart the game
    void OnEnable()
    {
        if (gameOver) RestartGame();        // reset on subsequent games only
    }

    // ends the game
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

    public void Pause()
    {
        player.enabled = false;
    }

    public void Resume()
    {
        player.enabled = true;
    }


    // ================================
    // Event Handlers
    // ================================

    // runs the turn initiated by the player being released
    private void HandlePlayerReleasedOnBoard(Vector3 position, int color)
    {
        Vector3 newPosition;
        bool validPosition = board.TryGetPlayerPosition(position, out newPosition);

        if (!validPosition)                        
        {
            player.ReturnToStart();
            return;
        }

        player.SnapToBoard(newPosition);                                    // render the snapping movement to the board

        int pointsScored = board.RunPlay(newPosition, color);            // runs all board logic
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

    // updating this function to instantiate the player, instead of player generator
    private void SpawnNewPlayer()
    {
        var playerColors = picker.CalculateNewPlayerColors();
        nextPlayerImage.SetSprite(playerColors.nextColor);

        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        player.Initialize(playerColors.color, board.GetBounds(), RowSize);

        player.PlayerReleasedOnBoard += HandlePlayerReleasedOnBoard;
    }

    private void RemoveCurrentPlayer()
    {
        player.PlayerReleasedOnBoard -= HandlePlayerReleasedOnBoard;        
        Destroy(player.gameObject);
    }
}
