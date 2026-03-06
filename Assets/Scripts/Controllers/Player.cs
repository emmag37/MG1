/**
 * Insert File Description
 * 
 */

using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    // ================================
    // Events
    // ================================
    public event Action<Vector3, int> PlayerReleasedOnBoard;

    // ================================
    // Private Fields
    // ================================
    private GamePieceImage image;
    private PlayerMovement movement;

    private int color;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        image = GetComponent<GamePieceImage>();
        movement = GetComponent<PlayerMovement>();

        movement.PlayerReleased += HandlePlayerRealeased;
    }


    // ================================
    // Initialize and Access Methods
    // ================================

    // add summaries
    public void Initialize(int newColor, Bounds boardBounds, int boardRowSize)
    {
        color = newColor;
        image.SetSprite(color);

        float radius = image.GetRadius();         // adjust the board boundaries to the player size
        float left = boardBounds.min.x + radius;
        float right = boardBounds.max.x - radius;
        float top = boardBounds.max.y - radius;

        movement.Initialize(left, right, top);
    }
    

    // ================================
    // Event Handlers
    // ================================
    public void HandlePlayerRealeased(Vector3 position)
    {
        PlayerReleasedOnBoard?.Invoke(position, color);
    }


    // ================================
    // Public Methods
    // ================================

    // add a summary
    public void SnapToBoard(Vector3 position)
    {
        movement.SnapToPosition(position);
    }

    // add a summary
    public void ReturnToStart()
    {
        movement.ReturnToStart();
    }
}
