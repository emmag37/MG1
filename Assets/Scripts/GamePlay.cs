using UnityEngine;

public class GamePlay : MonoBehaviour
{
    // children objects
    private Board board;
    private PlayerGenerator player_gen;

    // private variables
    private Player player;      // current active player
    private Bounds b;           // grid bounds


    void Awake()
    {
        // cache the children objects
        board = transform.GetChild(0).GetComponent<Board>();
        player_gen = transform.GetChild(1).GetComponent<PlayerGenerator>();

        // cache the boundaries
        b = board.GetComponent<SpriteRenderer>().bounds;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = Spawn();   // first player, enabled to move
    }

    // Update is called once per frame
    void Update()
    {
        player.Move();      // control the player from this script
    }

    private Player Spawn()
    {
        Player new_player = player_gen.SpawnPlayer();
        new_player.SetBoundaries(b.min.x, b.max.x, b.max.y);
        new_player.PlayerOnGrid += HandlePlayerOnGrid;       // enable to listen for event - remember to decrement when you disable player

        return new_player;
    }

    private void HandlePlayerOnGrid(Player player)
    {
        Debug.Log("handle player on grid");
        //  alert the board
    }
}
