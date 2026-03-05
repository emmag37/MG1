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
    public event Action<Vector2Int> PlayerReleasedOnBoard;

    // ================================
    // Private Fields
    // ================================
    private GamePieceImage image;
    private PlayerMovement movement;

    private int rowSize;
    private Vector2Int boardPos;

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
    public void Initialize(int color, Bounds boardBounds, int boardRowSize)
    {
        rowSize = boardRowSize;

        image.SetSprite(color);
        movement.InitializeBoundaries(boardBounds, image.GetSpriteBounds().extents.x, rowSize);
    }

    public int GetSpriteColor()
    {
        return image.GetNum();
    }

    // ================================
    // Event Handlers
    // ================================
    public void HandlePlayerRealeased(Vector2Int index)
    {
        if (CheckOnBoard(index.x, index.y))
        {
            boardPos = index;
            PlayerReleasedOnBoard?.Invoke(boardPos);
        } else
        {
            movement.ReturnToStart();
        }

    }

    // ================================
    // Public Methods
    // ================================

    // add a summary
    public void SnapToBoard()
    {
        movement.SnapToBoard(boardPos);
    }

    // add a summary
    public void ReturnToStart()
    {
        movement.ReturnToStart();
    }


    // ================================
    // Private Methods
    // ================================
    
    private bool CheckOnBoard(int row, int col)
    {
        return (row >= 0 && row <= rowSize - 1) && (col >= 0 && col <= rowSize - 1);
    }

    
}
