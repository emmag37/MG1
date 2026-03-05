/**
 * Insert File Description
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System;

public class GamePlay : MonoBehaviour
{
    // ================================
    // Events
    // ================================
    public event Action<int> GameOver;
    public event Action<int> UpdateScore;

    // ================================
    // Prefabs
    // ================================
    public Player playerPrefab;

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GamePieceImage nextPlayerImage;

    // ================================
    // Private Fields
    // ================================
    private Board board;
    private PlayerPicker picker;
    private Player player;                  // current active player
    private Bounds boardBounds;             // grid bounds (for the player)

    private bool gameOver = false;
    private int score;                      // current game score

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        // cache the children objects
        board = transform.GetChild(0).GetComponent<Board>();
        picker = new PlayerPicker();

        // cache the boundaries
        boardBounds = board.GetComponent<SpriteRenderer>().bounds;

        // subscribe to events
        board.BoardFull += HandleBoardFull;
    }

    // use to start/restart the game
    void OnEnable()
    {
        // reset if the game has run before
        if (gameOver) RestartGame();
    }

    void OnDisable()
    {
        if (player == null) return;

        // destroy the player
        player.PlayerReleasedOnBoard -= HandlePlayerReleasedOnBoard;
        Destroy(player.gameObject);

        // set game over
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
        // simply disable the player
        player.enabled = false;
    }

    public void Resume()
    {
        // enable the player
        player.enabled = true;
    }

    // ================================
    // Event Handlers
    // ================================

    // Function is called when the player is released - resets each frame
    // Essentially manages all of the game play actions, could clean this up with more helpers
    private void HandlePlayerReleasedOnBoard(Vector2Int boardIndex)
    {
        // check if the player is on an available spot on the grid
        if (board.IsFilled(boardIndex))
        {
            player.ReturnToStart();
            return;
        }

        // add player to the grid
        player.SnapToBoard();

        // the board checks for filled rows and returns the points scored during the turn
        int points = board.AddToBoard(boardIndex, player.GetSpriteNum());

        // update the score text
        if (points > 0)
        {
            score += points;
            UpdateScore?.Invoke(score);
        }

        // remove player - always remove even if game over
        player.PlayerReleasedOnBoard -= HandlePlayerReleasedOnBoard;
        Destroy(player.gameObject);

        // check for a game over
        if (gameOver)
        {
            // throw event to the game manager
            GameOver?.Invoke(score);
            return; // don't respawn
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
        // reset the game play
        gameOver = false;
        board.Reset();
        picker.Reset();
        score = 0;

        // spawn the first player
        SpawnNewPlayer();
    }

    // updating this function to instantiate the player, instead of player generator
    private void SpawnNewPlayer()
    {
        int nextColor = picker.GetNextPlayerColor();    // must call this BEFORE new color
        nextPlayerImage.SetSprite(nextColor);

        int color = picker.GetNewPlayerColor();

        player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation); // need to make sure the initial position is correct
        player.Initialize(color, boardBounds);

        player.PlayerReleasedOnBoard += HandlePlayerReleasedOnBoard;       // enable to listen for event - remember to decrement when you disable player
    }

    
}
