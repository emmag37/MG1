using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    // Prefabs
    public GameObject player;

    // Sprites - inspector
    // 0: color1, 1: color2, 2: color3, 3: color4, 4: color5, 5: color6, 6: wildcard
    public Sprite[] sprites;
    public GameObject nextPlayer;

    // private variables
    private int num;    // holds the value of the next color to be generated
    private SpriteRenderer sr;
    
    void Awake()
    {
        // Cache the values
        sr = nextPlayer.GetComponent<SpriteRenderer>();
    }

    
    void OnEnable()
    {
        // sets up the first player
        num = Random.Range(0, 6);   // removed WC for now
    }

    /* Creates a new player. */
    public Player SpawnPlayer()
    {
        Sprite color = sprites[num];

        // instantiate a new player and set its sprite
        GameObject newPlayer = Instantiate(player, transform.position, transform.rotation);
        Player np = newPlayer.GetComponent<Player>();
        np.SetSprite(color, num);

        SetNextPlayer();

        return np;
    }

    /* Sets the next gameobject and color */
    private void SetNextPlayer()
    {
        // set the next color
        num = Random.Range(0, 6);   // removed WC for now

        // set the sprite for the next color
        Sprite color = sprites[num];
        sr.sprite = color;
    }
}
