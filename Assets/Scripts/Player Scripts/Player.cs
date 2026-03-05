/**
 * Insert File Description
 * 
 */

using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private static readonly Vector2Int NotOnBoard = new Vector2Int(-1, -1);
    private static readonly Vector2Int NotOnGrid = new Vector2Int(-3, -3);

    // ================================
    // Events
    // ================================
    public event Action<Vector2Int> PlayerReleasedOnBoard;

    // ================================
    // Private Fields
    // ================================
    private Camera cam;
    private GamePieceImage image;

    private bool isDragging = false;
    private Vector3 dragOffset;

    private float minX, maxX, minY, maxY;   // boundary variables
    private float radius;
    
    private float cellOffset;               // grid calculation variables
    private Vector3 originCellPos;          

    private Vector3 startPos;
    private Vector2Int gridPos;             // index on coord sys centered at origin

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        cam = Camera.main;
        image = GetComponent<GamePieceImage>();

        minY = transform.position.y;
        radius = image.GetSpriteBounds().extents.x;

        startPos = transform.position;
        gridPos = NotOnGrid;
    }

    void Update()
    {
        Move();
    }

    // ================================
    // Initialize and Access Methods
    // ================================

    // add summaries
    public void Initialize(int color, Bounds boardBounds)
    {
        image.SetSprite(color);
        InitializeBoundaries(boardBounds.min.x, boardBounds.max.x, boardBounds.max.y);
    }

    public int GetSpriteNum()
    {
        return image.GetNum();
    }


    // ================================
    // Public Methods
    // ================================

    // add description - maybe even simplify this further
    // why is this public??
    public void Move()
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

            Vector2Int boardIndex = CheckOnBoard();
            if (boardIndex == NotOnBoard)
            {
                ReturnToStart();
            }
            else
            {
                PlayerReleasedOnBoard?.Invoke(boardIndex);      // throw event to the game manager
            }
        }
    }

    // add a summary
    public void SnapToBoard()
    {
        Vector3 newTransform = transform.position;

        newTransform.y = originCellPos.y + gridPos.x * cellOffset;
        newTransform.x = originCellPos.x + gridPos.y * cellOffset;

        transform.position = newTransform;
    }

    // add a summary
    public void ReturnToStart()
    {
        transform.position = startPos;
    }


    // ================================
    // Private Methods
    // ================================

    private void InitializeBoundaries(float boardLeft, float boardRight, float boardTop)
    {
        minX = boardLeft + radius;
        maxX = boardRight - radius;
        maxY = boardTop - radius;

        float gridWidth = boardRight - boardLeft;
        float spacing = (gridWidth - radius * 10) / 6;      // magic number

        cellOffset = radius * 2 + spacing;
        originCellPos = new Vector3(boardRight - gridWidth / 2, boardTop - gridWidth / 2, 0);
    }

    // returns the cell the player is hovering on, else returns (-1, -1)
    // change this to set an internal position(useful for the transform math),
    // but return a position usable by other game objects
    private Vector2Int CheckOnBoard()
    {
        int row = Mathf.RoundToInt((transform.position.y - originCellPos.y) / cellOffset);
        int col = Mathf.RoundToInt((transform.position.x - originCellPos.x) / cellOffset);

        if ((row >= -2 && row <= 2) && (col >= -2 && col <= 2)) // check for a grid index - magic numbers, get rid of these
        {
            gridPos.x = row;
            gridPos.y = col;

            return GridPosToBoardIndex(gridPos);
        }

        return NotOnBoard;
    }

    // converts the world row, col to the grid index
    private Vector2Int GridPosToBoardIndex(Vector2Int pos)
    {
        Vector2Int index = new Vector2Int();

        index.x = (pos.x * -1) + 2;   // reverse row direction first
        index.y = pos.y + 2;

        return index;
    }
}
