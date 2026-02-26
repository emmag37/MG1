using UnityEngine;

public class GameManager : MonoBehaviour
{
    // private variables
    [SerializeField] private GamePlay game_play;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to the game play events
        game_play.GameOver += HandleGameOver;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over");
    }
}
