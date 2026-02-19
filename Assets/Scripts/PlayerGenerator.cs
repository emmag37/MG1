using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    // Outside Objects - inspector
    public Player player;

    // Sprites - inspector
    public Sprite color1;
    public Sprite color2;
    public Sprite color3;
    public Sprite color4;
    public Sprite color5;
    public Sprite color6;
    public Sprite wildcard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Creates a new player. */
    void spawnPlayer()
    {
        // use a random number to pick the sprite
        int num = Random.Range(1, 8);
        Debug.Log("random number: " + num);

        Sprite color;
        switch (num)
        {
            case 1:
                color = color1;
                break;
            case 2:
                color = color2;
                break;
            case 3:
                color = color3;
                break;
            case 4:
                color = color4;
                break;
            case 5:
                color = color5;
                break;
            case 6:
                color = color6;
                break;
            case 7:
                color = wildcard;
                break;
            default:
                color = null;
                break;
        }
        Debug.Log(color ? "sprite: " + color.name : "No sprite selected");

        // instantiate a new player

    }
}
