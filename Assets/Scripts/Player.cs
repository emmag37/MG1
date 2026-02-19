using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gridSprite;

    // Components
    private SpriteRenderer sr;
    private Camera cam;

    // Move Variables
    private bool isDragging = false;
    private Vector3 dragOffset;
    private float minX, maxX, minY, maxY;

    void Awake()
    {
        // get components
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        // define boundaries
        Bounds gridBounds = gridSprite.bounds;
        float radius = sr.bounds.extents.x;

        minX = gridBounds.min.x + radius;
        maxX = gridBounds.max.x - radius;
        minY = transform.position.y;
        maxY = gridBounds.max.y;    // this is inaccurate - maybe scene object vs prefab?
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    /* Set the player's sprite to a new one */
    public void SetSprite(Sprite newSprite)
    {
        sr.sprite = newSprite;
    }

    private void Move()
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
        }
    }
}
