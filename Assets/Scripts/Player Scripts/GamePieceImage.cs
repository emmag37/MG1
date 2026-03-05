/**
 * Insert File Description
 * 
 */

using UnityEngine;

public class GamePieceImage : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int sortLayer = 1;
    private const int emptyColor = 0;

    // ================================
    // Inspector Fields
    // ================================
    // 0: empty, 1: color1, 2: color2, 3: color3, 4: color4, 5: color5, 6: color6, 7: wildcard
    public Sprite[] sprites;

    [SerializeField] private SpriteRenderer gamePieceRenderer;

    // ================================
    // Private Fields
    // ================================
    private int spriteColor;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        gamePieceRenderer = GetComponent<SpriteRenderer>();
    }

    // ================================
    // Access Methods
    // ================================

    /* Set the player's sprite to a new one, and updates the layer for visibility */
    // need to add one to the player picker
    public void SetSprite(int color)
    {
        gamePieceRenderer.sprite = sprites[color];
        gamePieceRenderer.sortingOrder = sortLayer;

        spriteColor = color;
    }

    public int GetNum()
    {
        return spriteColor;
    }

    public Bounds GetSpriteBounds()
    {
        return gamePieceRenderer.bounds;
    }

    // add short summary
    public void ResetPiece()
    {
        gamePieceRenderer.sprite = sprites[emptyColor];
        spriteColor = emptyColor;
    }


}
