using UnityEngine;

/// <summary>
/// Manages the visuals for players and board cells.
/// </summary>
public class GamePieceImage : MonoBehaviour
{
    // ================================
    // Constants
    // ================================

    private const int EmptyColor = 0;

    // ================================
    // Public Properties
    // ================================

    /// <summary>
	/// Radius of the game object.
	/// </summary>
    public float Radius { get; private set; }

    // ================================
    // Inspector Fields
    // ================================

    // 0: empty, 1: color1, 2: color2, 3: color3, 4: color4, 5: color5, 6: color6, 7: wildcard
    [SerializeField] private SpriteDatabase spriteD8;
    [SerializeField] private SpriteRenderer gamePieceRenderer;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        gamePieceRenderer = GetComponent<SpriteRenderer>();
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Sets the objects sprite to the given color.
	/// </summary>
	/// <param name="color">Color id of the sprite.</param>
	/// <remarks>Accesses the sprite from the sprite database.</remarks>
    public void SetSprite(int color)
    {
        gamePieceRenderer.sprite = spriteD8.sprites[color];
    }

    /// <summary>
	/// Changes the image to the 'empty' sprite.
	/// </summary>
    public void ResetPiece()
    {
        gamePieceRenderer.sprite = spriteD8.sprites[EmptyColor];
    }


}
