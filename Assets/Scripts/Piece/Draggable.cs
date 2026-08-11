using UnityEngine;
using UnityEngine.InputSystem;
using System;

// draggable within specified boundaries adjusted for the objects size

/// <summary>
/// Performs object movement operations including user input dragging
/// and dropping the object at specified position.
/// 
/// </summary>
/// <remarks>
/// When enabled, the user can drag the object.
/// </remarks>
///
// or do you require the sprite renderer component?
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    // ================================
    // Events
    // ================================

    public event Action StartDrag;

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

    void Update()
    {
        Drag();
    }


    // ================================
    // Initialization
    // ================================

    public void Initialize(Bounds boundaries, Camera cam)
    {
        this.cam = cam;
        if (cam == null)
            Debug.LogError("[Draggable] Camera not found");

        // adjust the boundaries to the player size
        float radius = GetComponent<SpriteRenderer>().bounds.extents.x;

        minX = boundaries.min.x + radius;
        maxX = boundaries.max.x - radius;
        minY = boundaries.min.y;
        maxY = boundaries.max.y - radius;
    }


    // ================================
    // Private Methods
    // ================================

    // add event for start drag

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

                StartDrag?.Invoke();
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
