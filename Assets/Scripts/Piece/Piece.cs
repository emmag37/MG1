using UnityEngine;
using System;


[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Draggable))]
[RequireComponent(typeof(Animator))]
public class Piece : MonoBehaviour, ILinkable<Piece, PieceData>
{
    // ==================================================
    // Local Events
    // ==================================================
    public event Action<Piece> PopFinished;

    // ==================================================
    // Public Properties
    // ==================================================
    public CellColor Color;
    public bool IsPlayer = false;

    public Piece Next { get; set; }
    public Piece Prev { get; set; }

    // ==================================================
    // Private Fields
    // ==================================================
    private SpriteRenderer spriteRenderer;
    private Draggable draggable;
    private Animator animator;

    private ISpriteDatabase spriteDatabase;

    private Vector3 spawnPoint;


    // ==================================================
    // Initialization
    // ==================================================

    public void Initialize(PieceData data)
    {
        spawnPoint = data.spawnPoint;

        Next = null;
        Prev = null;

        spriteRenderer = GetComponent<SpriteRenderer>();
        draggable = GetComponent<Draggable>();
        animator = GetComponent<Animator>();

        spriteDatabase = ServiceLocator.Get<ISpriteDatabase>();

        draggable.Initialize(data.boundaries, Camera.main);
        draggable.enabled = false;
    }

    public void SetToPlayer(CellColor playerColor)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("[Piece] Components must be initialized before setting to player");
            return;
        }

        if (!IsPlayer)
        {
            draggable.enabled = true;
            spriteRenderer.sortingOrder = GameConstants.PlayerOrder;

            draggable.StartDrag += HandleStartDrag;
            draggable.Released += HandleReleased;

            IsPlayer = true;
        }

        SetColor(playerColor);
        transform.position = spawnPoint;
    }

    public void SetToCell(CellColor color, Vector3 position)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("[Piece] Components must be initialized before setting to cell");
            return;
        }

        if (IsPlayer) TurnOffPlayer();          // ensure no event subscription

        SetColor(color);
        transform.position = position;
    }


    // ==================================================
    // Public Methods
    // ==================================================

    public void SetColor(CellColor newColor)
    {
        if (newColor == CellColor.Empty)
        {
            Debug.LogWarning("[Piece] Cannot set piece to empty color");
            return;
        }

        Color = newColor;
        spriteRenderer.sprite = spriteDatabase.GetSprite((int)Color);
    }

    public void PlacePlayer(Vector3 position)
    {
        Debug.Assert(IsPlayer, "[Piece] Attempted to place inactive player");

        transform.position = position;
        TurnOffPlayer();
    }

    public void ReturnPlayer()
    {
        Debug.Assert(IsPlayer, "[Piece] Attempted to return inactive player");

        transform.position = spawnPoint;
    }

    public void Pop()
    {
        Debug.Assert(!IsPlayer, "[Piece] Attempted pop animation on active player");

        animator.SetTrigger("PopCell");
    }

    public void Pause(bool pause)
    {
        Debug.Assert(IsPlayer, "[Piece] Attempted to pause/resume inactive player");

        draggable.enabled = !pause;       // true: stop moving, false: start moving
    }

    // ==================================================
    // Local Event Handlers
    // ==================================================

    private void HandleStartDrag()
    {
        ServiceLocator.Get<IAudio>().PlaySoundEffect(AudioType.PickupPlayer);
        EventBus.Publish(new PlayerDraggingEvent { PlayerTransform = transform, Color = Color });
    }

    private void HandleReleased(Vector3 position)
    {
        EventBus.Publish(new PlayerReleasedEvent { PlayerPosition = position, Color = Color });
    }

    private void OnAnimationComplete()
    {
        PopFinished?.Invoke(this);
    }
    

    // ==================================================
    // Private Methods
    // ==================================================

    private void TurnOffPlayer()
    {
        draggable.StartDrag -= HandleStartDrag;
        draggable.Released -= HandleReleased;

        draggable.enabled = false;
        spriteRenderer.sortingOrder = GameConstants.CellOrder;

        IsPlayer = false;
    }
}
