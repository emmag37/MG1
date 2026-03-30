using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(SpriteView))]
[RequireComponent(typeof(Draggable))]
public class PlayerView : MonoBehaviour
{
    // ================================
    // Private Fields
    // ================================
    private PlayerController controller;
    
    private SpriteView image;
    private Draggable movement;

    private Vector3 startPos;

    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        controller = GetComponent<PlayerController>();

        image = GetComponent<SpriteView>();
        movement = GetComponent<Draggable>();

        startPos = transform.position;

        movement.StartDrag += HandleStartDrag;
        movement.Released += HandleReleased;
    }

    void OnEnable()
    {
        EventBus.Subscribe<ReturnPlayerEvent>(OnReturnPlayer);
        EventBus.Subscribe<PlacePlayerEvent>(OnPlacePlayer);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<ReturnPlayerEvent>(OnReturnPlayer);
        EventBus.Unsubscribe<PlacePlayerEvent>(OnPlacePlayer);
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Initializes a player to be moved around the board and sets its color.
	/// </summary>
	/// <param name="color">Color id of the player.</param>
	/// <param name="boundaries">Boundaries of the board.</param>
    public void Initialize(CellColor playerColor, Bounds boundaries)
    {
        image.SetSprite(SpriteDatabase.Instance.GetSprite(playerColor));

        float radius = image.Radius;

        float left = boundaries.min.x + radius;     // adjust the board boundaries to the player size
        float right = boundaries.max.x - radius;
        float top = boundaries.max.y - radius;
        float bottom = boundaries.min.y;

        movement.Initialize(left, right, top, bottom);
    }


    // ================================
    // Event Bus Methods
    // ================================

    private void OnReturnPlayer(ReturnPlayerEvent e)
    {
        movement.Drop(startPos);
    }

    private void OnPlacePlayer(PlacePlayerEvent e)
    {
        movement.Drop(e.PlayerPosition);
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
