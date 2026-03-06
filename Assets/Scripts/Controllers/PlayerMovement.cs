/**
 * Insert File Description
 * 
 */

using UnityEngine;
using UnityEngine.InputSystem;
using System;

// edits to do:
    // change this class so that it ONLY moves the player
    // does not know anything about the grid

public class PlayerMovement : MonoBehaviour
{
    // ================================
    // Events
    // ================================

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
        Move();
    }


    // ================================
    // Initialization
    // ================================

    public void Initialize(float left, float right, float top)
    {
        minX = left;
        maxX = right;
        minY = startPos.y;
        maxY = top;
    }


    // ================================
    // Public Methods
    // ================================

    // add a summary
    public void SnapToPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    // add a summary
    public void ReturnToStart()
    {
        transform.position = startPos;
    }


    // ================================
    // Private Methods
    // ================================

    private void Move()
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
            PlayerReleased?.Invoke(transform.position);      // throw event to the player
        }
    }
}
