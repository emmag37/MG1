/**
 * Insert File Description
 * 
 */

using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private static readonly Vector2Int NotOnBoard = new Vector2Int(-1, -1);

    // ================================
    // Events
    // ================================
    public event Action<Vector2Int> PlayerReleasedOnBoard;

    // ================================
    // Private Fields
    // ================================
    private GamePieceImage image;
    private PlayerMovement movement;

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
    public void Initialize(int color, Bounds boardBounds)
    {
        image.SetSprite(color);
        movement.InitializeBoundaries(boardBounds.min.x, boardBounds.max.x, boardBounds.max.y, image.GetSpriteBounds().extents.x);
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
    
    // returns the cell the player is hovering on, else returns (-1, -1)
    // change this to set an internal position(useful for the transform math),
    // but return a position usable by other game objects
    private bool CheckOnBoard(int row, int col)
    {
        return (row >= 0 && row <= 4) && (col >= 0 && col <= 4);
    }

    
}
