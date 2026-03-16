using UnityEngine;

// MAKE MORE REUSABLE
    // Can add other useful measurements like height, width, for non circular objects

/// <summary>
/// Manages the sprite view.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteView : MonoBehaviour
{
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
    [SerializeField] private SpriteRenderer spriteRenderer;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateRadius();
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Sets the objects sprite to the given color.
	/// </summary>
	/// <param name="color">Color id of the sprite.</param>
	/// <remarks>Accesses the sprite from the sprite database.</remarks>
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        UpdateRadius();
    }


    // ================================
    // Private Methods
    // ================================

    private void UpdateRadius()
    {
        Radius = spriteRenderer.sprite.bounds.extents.x;
    }
}
