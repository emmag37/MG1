using UnityEngine;

public class GamePlay : MonoBehaviour
{
    // children objects
    private Board board;
    private PlayerGenerator player_gen;

    // private variables
    private Player player;      // current active player

    private float minX, maxX, maxY;


    void Awake()
    {
        // cache the children objects
        board = transform.GetChild(0).GetComponent<Board>();
        player_gen = transform.GetChild(1).GetComponent<PlayerGenerator>();

        // cache the boundaries
        Bounds b = board.GetComponent<SpriteRenderer>().bounds;
        minX = b.min.x;
        maxX = b.max.x;
        maxY = b.max.y;

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // generate the first player and set its
        player = Spawn();

    }

    // Update is called once per frame
    void Update()
    {
        player.Move();
    }

    private Player Spawn()
    {
        Player new_player = player_gen.SpawnPlayer();
        new_player.SetBoundaries(minX, maxX, maxY);

        return new_player;
    }
}
