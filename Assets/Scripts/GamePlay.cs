/**
 * Insert File Description
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System;

public class GamePlay : MonoBehaviour
{
    // events
    public event Action<int> GameOver;
    public event Action<int> UpdateScore;

    // children objects
    private Board board;
    private PlayerGenerator player_gen;

    // private variables
    private Player player;      // current active player
    private Bounds b;           // grid bounds
    private bool game_over = false;

    // score variables
    private int score;      // current game score

    void Awake()
    {
        // cache the children objects
        board = transform.GetChild(0).GetComponent<Board>();
        player_gen = transform.GetChild(1).GetComponent<PlayerGenerator>();

        // cache the boundaries
        b = board.GetComponent<SpriteRenderer>().bounds;

        // subscribe to events
        board.BoardFull += HandleBoardFull;
    }

    // use to start/restart the game
    void OnEnable()
    {
        // reset if the game has run before
        if (game_over) RestartGame();
    }

    void OnDisable()
    {
        if (player == null) return;

        // destroy the player
        player.PlayerReleased -= HandlePlayerReleased;
        Destroy(player.gameObject);

        // set game over
        game_over = true;
    }

    void Start()
    {
        player = Spawn();
    }

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

    private void RestartGame()
    {
        // reset the game play
        game_over = false;
        board.Reset();
        score = 0;

        // spawn the first player
        player = Spawn();
    }

    private Player Spawn()
    {
        Player new_player = player_gen.SpawnPlayer();
        new_player.SetBoundaries(b.min.x, b.max.x, b.max.y);
        new_player.PlayerReleased += HandlePlayerReleased;       // enable to listen for event - remember to decrement when you disable player

        return new_player;
    }

    // Function is called when the player is released - resets each frame
    // Essentially manages all of the game play actions, could clean this up with more helpers
    private void HandlePlayerReleased(Player curr_player)
    {
        if (curr_player != player)
        {
            return; // throw an exception or something?
        }

        // check if the player is on an available spot on the grid
        Vector2Int cell = player.OnCell();
        if (cell.x == -3 || board.IsFilled(cell))
        {
            player.ReturnToStart();
            return;
        }

        // add player to the grid
        player.SnapToCell(cell);

        // the board checks for filled rows and returns the points scored during the turn
        int points = board.AddToBoard(cell, player.GetSprite(), player.GetNum());

        // update the score text
        if (points > 0)
        {
            score += points;
            UpdateScore?.Invoke(score);
        }

        // remove player - always remove even if game over
        player.PlayerReleased -= HandlePlayerReleased;
        Destroy(player.gameObject);

        // check for a game over
        if (game_over)
        {
            // throw event to the game manager
            GameOver?.Invoke(score);
            return; // don't respawn
        }

        player = Spawn();
    }

    private void HandleBoardFull()
    {
        game_over = true;
    }
}
