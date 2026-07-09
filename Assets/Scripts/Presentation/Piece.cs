using UnityEngine;
using System;


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


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        EventBus.Subscribe<ReturnPlayerEvent>(OnReturnPlayer);
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);

        EventBus.Subscribe<PauseGameEvent>(OnPauseGame);
        EventBus.Subscribe<ResumeGameEvent>(OnResumeGame);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<ReturnPlayerEvent>(OnReturnPlayer);
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);

        EventBus.Unsubscribe<PauseGameEvent>(OnPauseGame);
        EventBus.Unsubscribe<ResumeGameEvent>(OnResumeGame);
    }


    // ==================================================
    // Initialization
    // ==================================================

    public void Initialize(CellColor playerColor, Bounds boundaries)
    {
        // initialize set values
        Color = playerColor;
        startPos = transform.position;

        // cache attached components
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
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

    public void Pop()
    {
        animator.SetTrigger("PopCell");
    }


    // ================================
    // Event Bus Methods
    // ================================

    private void OnReturnPlayer(ReturnPlayerEvent e)
    {
        dragAndDrop.Drop(startPos);
    }

    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        if (float.IsInfinity(e.PlayerPosition.x)) return;

        dragAndDrop.Drop(e.PlayerPosition);
        TurnOffPlayer();

        // make sure grid listens to this event and adds the object to the grid
    }

    private void OnPauseGame(PauseGameEvent e)
    {
        dragAndDrop.enabled = false;
    }

    private void OnResumeGame(ResumeGameEvent e)
    {
        Debug.Log("resume movement");
        dragAndDrop.enabled = true;
    }


    // ==================================================
    // Local Event Handlers
    // ==================================================

    private void HandleStartDrag()
    {
        EventBus.Publish(new PlayerDraggingEvent { PlayerTransform = transform, Color = Color });
    }

    private void HandleReleased(Vector3 position)
    {
        EventBus.Publish(new PlayerReleasedEvent { PlayerPosition = position, Color = Color });
    }

    private void OnAnimationComplete()
    {
        SetColor(CellColor.Empty);
        PopFinished?.Invoke(this);
    }


    // ================================
    // Private Methods
    // ================================

    private void TurnOffPlayer()    // leave this function for future additions, ie animations, sound effects
    {
        // turn off player aspects
        dragAndDrop.StartDrag -= HandleStartDrag;
        dragAndDrop.Released -= HandleReleased;
        dragAndDrop.enabled = false;

        EventBus.Unsubscribe<ReturnPlayerEvent>(OnReturnPlayer);
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);
        EventBus.Unsubscribe<PauseGameEvent>(OnPauseGame);
        EventBus.Unsubscribe<ResumeGameEvent>(OnResumeGame);

        // update the sorting order
        spriteRenderer.sortingOrder = 2;
    }
}
