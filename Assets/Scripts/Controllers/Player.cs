using UnityEngine;
using System;

/// <summary>
/// Manages the player in the game scene.
/// Handles the player movement and visual state.
/// </summary>
public class Player : MonoBehaviour
{
    // ================================
    // Events
    // ================================

    /// <summary>
	/// Invoked when the player is released.
	/// </summary>
    public event Action<Player> PlayerReleasedOnBoard;

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
    private GamePieceImage image;
    private PlayerMovement movement;

    private Vector3 startPos;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        image = GetComponent<GamePieceImage>();
        movement = GetComponent<PlayerMovement>();

        movement.PlayerReleased += HandlePlayerRealeased;

        startPos = transform.position;
    }


    // ================================
    // Initializers
    // ================================

    /// <summary>
	/// Initializes a player to be moved around the board and sets its color.
	/// </summary>
	/// <param name="color">Color id of the player.</param>
	/// <param name="boardBounds">Boundaries of the board.</param>
    public void Initialize(int color, Bounds boardBounds)
    {
        Color = color;
        image.SetSprite(color);

        float radius = image.Radius;

        // adjust the board boundaries to the player size
        float left = boardBounds.min.x + radius;
        float right = boardBounds.max.x - radius;
        float top = boardBounds.max.y - radius;

        movement.Initialize(left, right, top, startPos.y);
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Moves the player to a position and disables player movement.
	/// </summary>
	/// <param name="position">New position for the player.</param>
    public void SnapToBoard(Vector3 position)
    {
        movement.SnapToPosition(position);
        movement.enabled = false;
    }

    /// <summary>
	/// Returns the player to the start position.
	/// </summary>
    public void ReturnToStart()
    {
        movement.SnapToPosition(startPos);
    }


    // ================================
    // Event Handlers
    // ================================

    private void HandlePlayerRealeased(Vector3 position)
    {
        Position = position;
        PlayerReleasedOnBoard?.Invoke(this);
    }
}
