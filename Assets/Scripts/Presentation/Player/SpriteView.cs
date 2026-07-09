using UnityEngine;

// needs to be the ONLY script that accesses sprite database
    // one of my clean up goals

/// <summary>
/// Sets the player sprite view according to the sprite database.
/// </summary>
public class SpriteView : MonoBehaviour
{
    // ================================
    // Public Properties
    // ================================

    /// <summary>
    /// Radius of the game object.
    /// </summary>
    public float Radius { get; private set; }   // do not need this

    // ================================
    // Private Fields
    // ================================

    private SpriteRenderer spriteRenderer;


    // ================================
    // Public Methods
    // ================================

    public void Initialize(PlayerView player)
    {
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        SetSprite(SpriteDatabase.Instance.GetSprite(player.Color));

        Radius = spriteRenderer.bounds.extents.x;   // remove
    }

    // make this function private
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
