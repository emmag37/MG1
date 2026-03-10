using UnityEngine;
using UnityEngine.InputSystem;
using System;

/// <summary>
/// Performs all player movement operations including user input dragging
/// and moving the player to a specified position.
/// </summary>
/// <remarks>
/// When enabled, the user can drag the player.
/// </remarks>
public class PlayerMovement : MonoBehaviour
{
    // ================================
    // Events
    // ================================

    /// <summary>
	/// Invoked when the user releases the player.
	/// </summary>
    public event Action<Vector3> PlayerReleased;

    // ================================
    // Private Fields
    // ================================

    private Camera cam;

    private bool isDragging = false;
    private Vector3 dragOffset;

    private float minX, maxX, minY, maxY;

    private Vector3 startPos;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        cam = Camera.main;
        startPos = transform.position;
    }

    void Update()
    {
        MouseDrag();
    }


    // ================================
    // Initialization
    // ================================

    /// <summary>
	/// Sets the boundaries for dragging the player around the screen.
	/// </summary>
	/// <param name="left">Left boundary.</param>
	/// <param name="right">Right boundary.</param>
	/// <param name="top">Top boundary.</param>
	/// <param name="bottom">Bottom boundary.</param>
    public void Initialize(float left, float right, float top, float bottom)
    {
        minX = left;
        maxX = right;
        minY = bottom;
        maxY = top;
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Updates the player's transform.
	/// </summary>
	/// <param name="newPosition">New position for the player.</param>
    public void SnapToPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }


    // ================================
    // Private Methods
    // ================================

    // Drags and drops the player from user input. Relies on Update().
    private void MouseDrag()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();    // obtain the mouse world coordinates
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        // start moving
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos); // check if mouse is on the collider
            if (hit && hit.gameObject == gameObject)
            {
                isDragging = true;
                dragOffset = transform.position - mouseWorldPos;
            }
        }

        // continue moving
        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Vector3 newPos = mouseWorldPos + dragOffset;    // calculate new position

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);   // clamp position to boundaries
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
        }

        // release
        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            PlayerReleased?.Invoke(transform.position);      // throw event to the player script
        }
    }
}
