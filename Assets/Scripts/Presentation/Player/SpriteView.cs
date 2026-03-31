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
    // Private Fields
    // ================================

    private SpriteRenderer spriteRenderer;


    // ================================
    // Public Methods
    // ================================

    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Radius = spriteRenderer.bounds.extents.x;

    }

    /// <summary>
	/// Sets the objects sprite to the given color.
	/// </summary>
	/// <param name="color">Color id of the sprite.</param>
	/// <remarks>Accesses the sprite from the sprite database.</remarks>
    public void SetSprite(Sprite sprite)
    {
        Debug.Assert(sprite != null, "Attempted to set sprite to null");

        spriteRenderer.sprite = sprite;
        Radius = spriteRenderer.bounds.extents.x;
    }
}
