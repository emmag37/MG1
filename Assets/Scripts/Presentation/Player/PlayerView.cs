using UnityEngine;

[RequireComponent(typeof(PlayerController))]    // want to take this off of the player

[RequireComponent(typeof(Draggable))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerView : MonoBehaviour
{
    // ================================
    // Public Fields
    // ================================
    public CellColor Color;

    // ================================
    // Private Fields
    // ================================
    private PlayerController controller;    // remove this

    private Draggable dragAndDrop;          // turn more into just an input handler

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

    // ================================
    // Initializers
    // ================================

    // next step - fix the draggable initialization
        // draggable should take a sprite renderer and some boundaries

    /// <summary>
    /// Initializes a player to be moved around the board and sets its color.
    /// </summary>
    /// <param name="color">Color id of the player.</param>
    /// <param name="boundaries">Boundaries of the board.</param>
    public void Initialize(CellColor playerColor, Bounds boundaries)
    {
        // initialize set values
        Color = playerColor;
        startPos = transform.position;

        // cache attached components
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        dragAndDrop = GetComponent<Draggable>();

        // initialize your components
        spriteRenderer.sprite = SpriteDatabase.Instance.GetSprite(Color);
        dragAndDrop.Initialize(spriteRenderer, boundaries);

        // subscribe to events
        dragAndDrop.StartDrag += HandleStartDrag;
        dragAndDrop.Released += HandleReleased;

        // old code
        controller = GetComponent<PlayerController>();
    }


    // want to remove the event bus from the player
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


    // ================================
    // Private Methods
    // ================================

    private void HandleStartDrag()
    {
        controller.StartDrag();
    }

    private void HandleReleased(Vector3 position)
    {
        controller.Released(position);
    }
}
