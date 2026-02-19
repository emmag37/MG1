using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Components
    private SpriteRenderer sr;

    // Move Variables
    private bool isDragging = false;
    private Vector3 dragOffset;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
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
            transform.position = mouseWorldPos + dragOffset;
        }

        // release
        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;

            Debug.Log("stop dragging");
        }
    }
}
