using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    // Prefabs
    public GameObject player;

    // Sprites - inspector
    // 0: color1, 1: color2, 2: color3, 3: color4, 4: color5, 5: color6, 6: wildcard
    public Sprite[] sprites;

    /* Creates a new player. */
    void spawnPlayer()
    {
        // use a random number to pick the sprite
        int num = Random.Range(0, 7);
        Debug.Log("random number: " + num);

        Sprite color = sprites[num];
        Debug.Log(color ? "sprite: " + color.name : "No sprite selected");

        // instantiate a new player and set its sprite
        GameObject newPlayer = Instantiate(player, transform.position, transform.rotation);
        newPlayer.GetComponent<Player>().SetSprite(color);
        Debug.Log("generated player");

    }
}
