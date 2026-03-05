using UnityEngine;

public class PlayerImage : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int sortLayer = 1;

    // ================================
    // Inspector Fields
    // ================================
    // 0: color1, 1: color2, 2: color3, 3: color4, 4: color5, 5: color6, 6: wildcard, 7: empty
    public Sprite[] sprites;

    [SerializeField] private SpriteRenderer sr;

    // ================================
    // Private Fields
    // ================================
    private int spriteNum;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // ================================
    // Access Methods
    // ================================

    /* Set the player's sprite to a new one, and updates the layer for visibility */
    public void SetSprite(int color)
    {
        sr.sprite = sprites[color];
        sr.sortingOrder = sortLayer;

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
