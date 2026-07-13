using UnityEngine;
using System;

// script is clean and ready to remove event bus

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Draggable))]
[RequireComponent(typeof(Animator))]
public class Piece : MonoBehaviour
{
    // ==================================================
    // Local Events
    // ==================================================
    public event Action<Piece> PopFinished;

    // ==================================================
    // Public Properties
    // ==================================================
    public CellColor Color;

    // ==================================================
    // Private Fields
    // ==================================================
    private SpriteRenderer spriteRenderer;
    private Draggable dragAndDrop;
    private Animator animator;

    private Vector3 startPos;


    // ==================================================
    // Initialization
    // ==================================================

    public void Initialize(CellColor playerColor, Bounds boundaries)
    {
        // initialize set values
        Color = playerColor;
        startPos = transform.position;

        // cache attached components
        spriteRenderer = GetComponent<SpriteRenderer>();
        dragAndDrop = GetComponent<Draggable>();
        animator = GetComponent<Animator>();

        // initialize your components
        spriteRenderer.sprite = SpriteDatabase.Instance.GetSprite(Color);
        dragAndDrop.Initialize(spriteRenderer, boundaries);

        // subscribe to events
        dragAndDrop.StartDrag += HandleStartDrag;
        dragAndDrop.Released += HandleReleased;
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Sets the cell sprite to the given color.
	/// </summary>
	/// <param name="newColor">New color for the cell.</param>
    public void SetColor(CellColor newColor)
    {
        Debug.Assert(!dragAndDrop.enabled, "Attempted to set color on active player");

        switch (newColor)
        {
            case CellColor.Shadow:
                spriteRenderer.sprite = SpriteDatabase.Instance.GetShadow(Color);
                break;

            case CellColor.ResetShadow:
                spriteRenderer.sprite = SpriteDatabase.Instance.GetSprite(Color);
                break;

            default:
                Color = newColor;
                spriteRenderer.sprite = SpriteDatabase.Instance.GetSprite(Color);
                break;
        }
    }

    public void PlacePlayer(Vector3 position)
    {
        dragAndDrop.Drop(position);
        TurnOffPlayer();
    }

    public void ReturnPlayer()
    {
        dragAndDrop.Drop(startPos);
    }

    public void Pop()
    {
        Debug.Assert(!dragAndDrop.enabled, "Attempted pop animation on active player");

        animator.SetTrigger("PopCell");
    }

    // ==================================================
    // Local Event Handlers
    // ==================================================

    private void HandleStartDrag()
    {
        Debug.Assert(dragAndDrop.enabled, "Receiving input on inactive piece");

        EventBus.Publish(new PlayerDraggingEvent { PlayerTransform = transform, Color = Color });
    }

    private void HandleReleased(Vector3 position)
    {
        Debug.Assert(dragAndDrop.enabled, "Receiving input on inactive piece");

        EventBus.Publish(new PlayerReleasedEvent { PlayerPosition = position, Color = Color });
    }

    private void OnAnimationComplete()
    {
        SetColor(CellColor.Empty);

        PopFinished?.Invoke(this);

        Destroy(gameObject);    // eventually remove once you put in your object pool
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void TurnOffPlayer()    // leave this function for future additions, ie animations, sound effects
    {
        dragAndDrop.StartDrag -= HandleStartDrag;
        dragAndDrop.Released -= HandleReleased;
        dragAndDrop.enabled = false;

        spriteRenderer.sortingOrder = 2;    // object reference not set to instance of object
    }
}
