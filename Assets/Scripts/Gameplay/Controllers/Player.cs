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
[RequireComponent(typeof(SpriteView))]
[RequireComponent(typeof(Draggable))]
public class Player : MonoBehaviour
{
    // ================================
    // Private Fields
    // ================================
    private SpriteView image;
    private Draggable movement;

    private Vector3 startPos;
    private CellColor color;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        image = GetComponent<SpriteView>();
        movement = GetComponent<Draggable>();
        startPos = transform.position;

        movement.Released += HandleReleased;
    }

    void OnEnable()
    {
        // Board Events
        EventBus.Subscribe<PlacePlayerEvent>(HandlePlacePlayer);
        EventBus.Subscribe<ReturnPlayerEvent>(HandleReturnPlayer);
    }

    void OnDisable()
    {
        // Board Events
        EventBus.Unsubscribe<PlacePlayerEvent>(HandlePlacePlayer);
        EventBus.Unsubscribe<ReturnPlayerEvent>(HandleReturnPlayer);
    }

    // ================================
    // Initializers
    // ================================

    /// <summary>
	/// Initializes a player to be moved around the board and sets its color.
	/// </summary>
	/// <param name="color">Color id of the player.</param>
	/// <param name="boundaries">Boundaries of the board.</param>
    public void Initialize(CellColor playerColor, Bounds boundaries)
    {
        color = playerColor;
        image.SetSprite(SpriteDatabase.Instance.GetSprite(color));

        float radius = image.Radius;

        float left = boundaries.min.x + radius;     // adjust the board boundaries to the player size
        float right = boundaries.max.x - radius;
        float top = boundaries.max.y - radius;
        float bottom = boundaries.min.y;

        movement.Initialize(left, right, top, bottom);
    }

    // ================================
    // Event Handlers
    // ================================

    private void HandleReleased(Vector3 position)
    {
        EventBus.Publish(new PlayerReleasedEvent { PlayerPosition = position, Color = color });
    }

    private void HandlePlacePlayer(PlacePlayerEvent e)
    {
        movement.Drop(e.PlayerPosition);
    }

    private void HandleReturnPlayer(ReturnPlayerEvent e)
    {
        movement.Drop(startPos);
    }
}
