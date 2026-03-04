using UnityEngine;

public class PlayerImage : MonoBehaviour
{
    // Player Sprites - inspector
    // 0: color1, 1: color2, 2: color3, 3: color4, 4: color5, 5: color6, 6: wildcard
    public Sprite[] sprites;

    [SerializeField] private SpriteRenderer sr;
    private int spriteNum;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /* Set the player's sprite to a new one, and updates the layer for visibility */
    public void SetSprite(int color)
    {
        sr.sprite = sprites[color];
        sr.sortingOrder = 1;

        spriteNum = color;
    }

    public Sprite GetSprite()
    {
        return sprites[spriteNum];
    }

    public int GetNum()
    {
        return spriteNum;
    }

    public Bounds GetSpriteBounds()
    {
        return sr.bounds;
    }

}
