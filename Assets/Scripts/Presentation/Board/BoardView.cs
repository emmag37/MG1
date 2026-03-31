using UnityEngine;

public class BoardView : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int RowSize = GameConstants.RowSize;

    // ================================
    // Public Fields
    // ================================

    public Bounds BoardBounds => background.bounds;

    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private GridView gridView;

    // ================================
    // Private Fields
    // ================================
    private BoardGeometry geometry;
    private BoardController boardController;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(background != null, "Background not set in board view");
    }

    void Awake()
    {
        boardController = GetComponent<BoardController>();
        geometry = new BoardGeometry();

        gridView.Initialize();
        geometry.Initialize(gridView.CellRadius, BoardBounds);
    }

    void OnEnable()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);

        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);
    }


    // ================================
    // Public Methods
    // ================================
    // used by ghost preview

    public Vector2Int WorldToIndex(Vector3 position)
    {
        return geometry.TransformToBoardIndex(position);
    }

    public Vector3 IndexToWorld(Vector2Int index)
    {
        return geometry.BoardIndexToTransform(index);
    }

    public void SetCellColor(Vector2Int index, CellColor color)
    {
        gridView.SetCell(index, color);
    }

    // ================================
    // Game State Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        gridView.ResetCells();

        background.gameObject.SetActive(true);
        gridView.gameObject.SetActive(true);
    }

    private void OnExitGame(ExitGameEvent e)
    {
        background.gameObject.SetActive(true);
        gridView.gameObject.SetActive(true);
    }

    private void OnGameOver(GameOverEvent e)
    {
        background.gameObject.SetActive(true);
        gridView.gameObject.SetActive(true);
    }


    // ================================
    // Player Event Handlers
    // ================================

    private void OnPlayerReleased(PlayerReleasedEvent e)
    {
        Vector2Int index = geometry.TransformToBoardIndex(e.PlayerPosition);
        Vector3 newPosition = geometry.BoardIndexToTransform(index);

        boardController.TryPlacePlayer(index, e.Color, newPosition);
    }
}
