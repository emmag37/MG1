using UnityEngine;
using UnityEngine.InputSystem;
using System;

// draggable within specified boundaries adjusted for the objects size
// one touch at a time

// consider adding a layer mask for future projects, not necessary for this one

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

        if (minX >= maxX || minY >= maxY)                                  // verify boundaries
        {
            Debug.LogError($"[Draggable] Invalid boundaries: x ({minX}, {maxX}), y ({minY}, {maxY})");
        }
    }


    // ================================
    // Private Methods
    // ================================


    /// <summary>
	/// Drags and drops the player from user input. Relies on Update().
	/// </summary>
    private void Drag()
    {
        if (cam == null) return;

        Vector2 screenPos;
        bool pressedThisFrame, isPressed, releasedThisFrame;

        if (Touchscreen.current != null)                                
        {
            var t = Touchscreen.current.primaryTouch;                   // enforce one touch only
            screenPos = t.position.ReadValue();
            pressedThisFrame = t.press.wasPressedThisFrame;
            isPressed = t.press.isPressed;
            releasedThisFrame = t.press.wasReleasedThisFrame;
        }
        else if (Mouse.current != null)
        {
            var m = Mouse.current;                                      // for use in the editor
            screenPos = m.position.ReadValue();
            pressedThisFrame = m.leftButton.wasPressedThisFrame;
            isPressed = m.leftButton.isPressed;
            releasedThisFrame = m.leftButton.wasReleasedThisFrame;
        }
        else
            return;

        Vector3 pointerWorldPos = cam.ScreenToWorldPoint(screenPos);
        pointerWorldPos.z = 0;
        
        if (!isDragging && pressedThisFrame)                            // start moving
        {
            Collider2D hit = Physics2D.OverlapPoint(pointerWorldPos);   // check if mouse is on the collider
            if (hit && hit.gameObject == gameObject)
            {
                isDragging = true;
                dragOffset = transform.position - pointerWorldPos;

                StartDrag?.Invoke();
            }
        }
        else if (isDragging && isPressed)                               // continue moving
        {
            Vector3 newPos = pointerWorldPos + dragOffset;              // calculate new position

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);               // clamp position to boundaries
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
        }
        else if (isDragging && releasedThisFrame)                       // release
        {
            isDragging = false;
            Released?.Invoke(transform.position);      
        }
    }
}
