using UnityEngine;

public class GamePlay : MonoBehaviour
{
    // children objects
    private Board board;
    private PlayerGenerator player_gen;

    // private variables
    private Player player;      // current active player
    private Bounds b;           // grid bounds

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

    private Player Spawn()
    {
        Player new_player = player_gen.SpawnPlayer();
        new_player.SetBoundaries(b.min.x, b.max.x, b.max.y);
        new_player.PlayerReleased += HandlePlayerReleased;       // enable to listen for event - remember to decrement when you disable player

        return new_player;
    }

    // Function is called when the player is released - resets each frame
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

        // check for a game over first

        // remove player and respawn
        player.PlayerReleased -= HandlePlayerReleased;
        Destroy(player.gameObject);

        player = Spawn();
    }

    private void HandleBoardFull(Board board)
    {
        Debug.Log("Game Over!");
    }
}
