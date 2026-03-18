using UnityEngine;
using UnityEngine.InputSystem;
using System;

// IF YOU MAKE REUSABLE:
    // set camera as a serialized field
    // add event for drag started

/// <summary>
/// Performs object movement operations including user input dragging
/// and dropping the object at specified position.
/// 
/// </summary>
/// <remarks>
/// When enabled, the user can drag the object.
/// </remarks>
public class Draggable : MonoBehaviour
{
    // ================================
    // Events
    // ================================

    /// <summary>
	/// Invoked when the user releases the player.
	/// </summary>
    public event Action<Vector3> Released;

    // ================================
    // Private Fields
    // ================================

    private Camera cam;

    private bool isDragging = false;
    private Vector3 dragOffset;

    private float minX, maxX, minY, maxY;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        cam = Camera.main;
        Debug.Assert(cam != null, "Main camera not found");
    }

    void Update()
    {
        Drag();
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
	/// Updates the player's transform position.
	/// </summary>
	/// <param name="newPosition">New position for the player.</param>
    public void Drop(Vector3 newPosition)
    {
        transform.position = newPosition;
    }


    // ================================
    // Private Methods
    // ================================

    /// <summary>
	/// Drags and drops the player from user input. Relies on Update().
	/// </summary>
    private void Drag()
    {
        var pointer = Pointer.current;
        if (pointer == null) return;

        Vector2 pointerScreenPos = pointer.position.ReadValue();    // obtain the mouse world coordinates
        Vector3 pointerWorldPos = cam.ScreenToWorldPoint(pointerScreenPos);
        pointerWorldPos.z = 0;

        // start moving
        if (pointer.press.wasPressedThisFrame)
        {
            Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos); // check if mouse is on the collider
            if (hit && hit.gameObject == gameObject)
            {
                isDragging = true;
                dragOffset = transform.position - pointerWorldPos;
            }
        }

        // continue moving
        if (isDragging && pointer.press.isPressed)
        {
            Vector3 newPos = pointerWorldPos + dragOffset;    // calculate new position

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);   // clamp position to boundaries
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
        }

        // release
        if (isDragging && pointer.press.wasReleasedThisFrame)
        {
            isDragging = false;
            Released?.Invoke(transform.position);      // throw event to the player script
        }
    }
        
}
