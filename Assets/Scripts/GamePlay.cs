using UnityEngine;

public class GamePlay : MonoBehaviour
{
    // children objects
    private Board board;
    private PlayerGenerator player_gen;


    void Awake()
    {
        // cache the children objects
        board = transform.GetChild(0).GetComponent<Board>();
        player_gen = transform.GetChild(1).GetComponent<PlayerGenerator>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_gen.SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
