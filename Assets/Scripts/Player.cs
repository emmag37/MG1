using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Components
    private SpriteRenderer sr;
    private Camera cam;

    // Move Variables
    private bool isDragging = false;
    private Vector3 dragOffset;
    private float minX, maxX, minY, maxY;

    private float radius;
    private float cell_offset = 0;  // spacing + radius
    private Vector3 cell0_pos;      // origin
    private Vector3 start_pos;      // starting position

    void Awake()
    {
        // get components
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        minX = 0; maxX = 0; maxY = 0;
        minY = transform.position.y;
        radius = sr.bounds.extents.x; // half-width

        start_pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    /* Set the player's sprite to a new one, and updates the layer for visibility */
    public void SetSprite(Sprite newSprite)
    {
        sr.sprite = newSprite;
        sr.sortingOrder = 1;
    }

    public void SetBoundaries(float x1, float x2, float y)
    {
        // adjust these with the player's radius
        minX = x1 + radius;
        maxX = x2 - radius;
        maxY = y - radius;

        // use these for grid math
        float grid_width = maxX - minX;
        float spacing = (grid_width - radius * 10) / 6;
        cell_offset = radius * 2 + spacing;
        cell0_pos = new Vector3(maxX - grid_width/2, maxY - grid_width/2, 0);

        Debug.Log("set player boundaries and grid math");
    }
    
    public void Move()
    {
        if (Mouse.current == null) return;

        // obtain the mouse world coordinates
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        // start moving
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // check if mouse is on the collider
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
            if (hit && hit.gameObject == gameObject)
            {
                isDragging = true;
                dragOffset = transform.position - mouseWorldPos;    // define offset

                Debug.Log("start dragging");
            }
        }

        // continue moving
        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            Vector3 newPos = mouseWorldPos + dragOffset;    // calculate new position

            // clamp position to boundaries
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;
        }

        // release
        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            Debug.Log("stop dragging");

            // check if on cell
            Vector2 cell = isPlayerOnCell(transform.position);
            if (cell.x != -3)
            {
                SnapPlayerToCell(cell);
                // alert the game play manager with the cell it landed on

                Debug.Log("snapped player to cell");
            } else
            {
                // return to the start pos
                transform.position = start_pos;
            }
        }
    }

    // returns the cell the player is hovering on, else returns (-1, -1)
    private Vector2 isPlayerOnCell(Vector3 player_pos)
    {
        Vector2 grid_pos = new Vector2(-3, -3);     // default value for not on grid

        int row = Mathf.RoundToInt((player_pos.y - cell0_pos.y) / cell_offset);
        Debug.Log("row: " + row);

        int col = Mathf.RoundToInt((player_pos.x - cell0_pos.x) / cell_offset);
        Debug.Log("col: " + col);

        if ((row >= -2 && row <= 2) && (col >= -2 && col <= 2)) // make sure it's on the grid
        {
            grid_pos.x = row;
            grid_pos.y = col;
        }

        return grid_pos;
    }

    private void SnapPlayerToCell(Vector2 cell)
    {
        Vector3 new_pos = transform.position;

        new_pos.y = cell0_pos.y + cell.x * cell_offset;
        new_pos.x = cell0_pos.x + cell.y * cell_offset;

        transform.position = new_pos;
    }
}
