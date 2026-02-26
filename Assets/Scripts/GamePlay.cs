using UnityEngine;
using System;

public class GamePlay : MonoBehaviour
{
    // public variables
    public event Action GameOver;

    // children objects
    private Board board;
    private PlayerGenerator player_gen;

    // private variables
    private Player player;      // current active player
    private Bounds b;           // grid bounds
    private bool game_over = false;

    // move the release logic into the game play

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = Spawn();   // first player, enabled to move
    }

    // add a public pause function
    // add a public restart function

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
            Debug.Log("non active player released");
            return; // throw an exception or something?
        }

        Debug.Log("handle player released");

        // check if the player is on an available spot on the grid
        Vector2Int cell = player.OnCell();
        if (cell.x == -3 || board.IsFilled(cell))
        {
            Debug.Log("return player to start");
            player.ReturnToStart();
            return;
        }

        // add player to the grid - the board checks for filled rows
        player.SnapToCell(cell);
        board.SetFilled(cell, player.GetSprite(), player.GetNum());

        // remove player - always remove even if game over
        player.PlayerReleased -= HandlePlayerReleased;
        Destroy(player.gameObject);

        // check for a game over
        if (game_over)
        {
            // throw event to the game manager
            GameOver?.Invoke();
            return; // don't respawn
        }

        player = Spawn();
    }

    private void HandleBoardFull()
    {
        Debug.Log("Board Full");
        game_over = true;
    }
}
