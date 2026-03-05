/**
 * Insert File Description
 * 
 */

using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{
    // ================================
    // Events
    // ================================
    public event Action<Vector2Int> PlayerReleased;

    // ================================
    // Private Fields
    // ================================
    private Camera cam;

    private bool isDragging = false;
    private Vector3 dragOffset;

    private float minX, maxX, minY, maxY;   // boundary variables

    private float cellOffset;               // grid calculation variables
    private Vector3 originCellPos;
    private int rowSize;

    private Vector3 startPos;


    // ================================
    // Unity Lifecycle Methods
    // ================================
    void Awake()
    {
        cam = Camera.main;
        minY = transform.position.y;
        startPos = transform.position;
    }

    void Update()
    {
        Move();
    }

    // ================================
    // Public Methods
    // ================================
    public void InitializeBoundaries(Bounds board, float radius, int boardRowSize)
    {
        rowSize = boardRowSize;

        float boardLeft = board.min.x;
        float boardRight = board.max.x;
        float boardTop = board.max.y;

        minX = boardLeft + radius;
        maxX = boardRight - radius;
        maxY = boardTop - radius;

        float gridWidth = boardRight - boardLeft;
        float spacing = (gridWidth - radius * (rowSize * 2)) / (rowSize + 1);

        cellOffset = radius * 2 + spacing;
        originCellPos = new Vector3(boardRight - gridWidth / 2, boardTop - gridWidth / 2, 0);
    }

    // add a summary
    public void SnapToBoard(Vector2Int boardIndex)
    {
        Vector3 newTransform = transform.position;
        Vector2Int gridIndex = BoardToGridIndex(boardIndex);

        newTransform.y = originCellPos.y + gridIndex.x * cellOffset;
        newTransform.x = originCellPos.x + gridIndex.y * cellOffset;

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

            Vector2Int boardIndex = TransformToBoardIndex();
            PlayerReleased?.Invoke(boardIndex);      // throw event to the player
        }
    }

    // Index Calculations
    private Vector2Int TransformToBoardIndex()
    {
        Vector2Int gridIndex = new Vector2Int();

        gridIndex.x = Mathf.RoundToInt((transform.position.y - originCellPos.y) / cellOffset);
        gridIndex.y = Mathf.RoundToInt((transform.position.x - originCellPos.x) / cellOffset);

        return GridToBoardIndex(gridIndex);
    }

    private Vector2Int GridToBoardIndex(Vector2Int pos)
    {
        Vector2Int index = new Vector2Int();

        index.x = (rowSize - 1) / 2 - pos.x;   // reverse row direction first
        index.y = pos.y + (rowSize - 1) / 2;

        return index;
    }

    private Vector2Int BoardToGridIndex(Vector2Int pos)
    {
        Vector2Int index = new Vector2Int();

        index.x = (rowSize - 1) / 2 - pos.x;   // reverse row direction first
        index.y = pos.y - (rowSize - 1) / 2;

        return index;
    }

}
