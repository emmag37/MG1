using UnityEngine;
using System;

/// <summary>
/// Manages the player in the game scene.
/// Handles the player movement and visual state.
///
/// Can:
/// - Move the player within specified boundaries
/// - Maintain a sprite/color state for the image, relies on sprite database
/// - Enabling/disabling the player only affects user dragging
/// 
/// </summary>
public class Player : MonoBehaviour
{
    // ================================
    // Events
    // ================================

    /// <summary>
	/// Invoked when the player is released.
	/// </summary>
    public event Action<Player> PlayerReleased;

    // ================================
    // Public Properties
    // ================================

    /// <summary>
	/// Color of the player object.
	/// </summary>
    public int Color { get; private set; }

    /// <summary>
	/// Current position of the player.
	/// </summary>
    public Vector3 Position { get; private set; }


    // ================================
    // Private Fields
    // ================================
    private SpriteView image;
    private Draggable movement;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        image = GetComponent<SpriteView>();
        movement = GetComponent<Draggable>();
        Position = transform.position;

        movement.Released += HandlePlayerReleased;
    }


    // ================================
    // Initializers
    // ================================

    /// <summary>
	/// Initializes a player to be moved around the board and sets its color.
	/// </summary>
	/// <param name="color">Color id of the player.</param>
	/// <param name="boundaries">Boundaries of the board.</param>
    public void Initialize(int color, Sprite sprite, Bounds boundaries)
    {
        Color = color;
        image.SetSprite(sprite);

        float radius = image.Radius;

        float left = boundaries.min.x + radius;     // adjust the board boundaries to the player size
        float right = boundaries.max.x - radius;
        float top = boundaries.max.y - radius;
        float bottom = boundaries.min.y;

        movement.Initialize(left, right, top, bottom);
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Moves the player to a position.
	/// </summary>
	/// <param name="position">New position for the player.</param>
    public void Move(Vector3 position)
    {
        movement.Drop(position);
        Position = position;
    }


    // ================================
    // Event Handlers
    // ================================

    private void HandlePlayerReleased(Vector3 position)
    {
        Position = position;
        PlayerReleased?.Invoke(this);
    }
}
