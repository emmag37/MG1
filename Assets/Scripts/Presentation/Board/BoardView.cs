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
    [SerializeField] private BoardController boardController;

    [SerializeField] private SpriteRenderer background;
    [SerializeField] private GameObject cells;

    // ================================
    // Private Fields
    // ================================
    private Cell[,] grid = new Cell[RowSize, RowSize];  // grid children
    private BoardGeometry geometry;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(background != null, "Background not set in board view");
        Debug.Assert(cells != null, "Cells not set in board view");
    }

    void Awake()
    {
        geometry = new BoardGeometry();

        InitializeCells();
        geometry.Initialize(grid[0, 0].Radius, BoardBounds);

    }

    void OnEnable()
    {
        EventBus.Subscribe<StartGameEvent>(OnStartGame);
        EventBus.Subscribe<ExitGameEvent>(OnExitGame);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);

        EventBus.Subscribe<PlayerReleasedEvent>(OnPlayerReleased);

        EventBus.Subscribe<SetCellEvent>(OnSetCell);
        EventBus.Subscribe<ClearRowEvent>(OnClearRow);
        EventBus.Subscribe<ClearColumnEvent>(OnClearColumn);
        EventBus.Subscribe<ClearRightDiagEvent>(OnClearRightDiag);
        EventBus.Subscribe<ClearLeftDiagEvent>(OnClearLeftDiag);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<StartGameEvent>(OnStartGame);
        EventBus.Unsubscribe<ExitGameEvent>(OnExitGame);
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);

        EventBus.Unsubscribe<PlayerReleasedEvent>(OnPlayerReleased);

        EventBus.Unsubscribe<SetCellEvent>(OnSetCell);
        EventBus.Unsubscribe<ClearRowEvent>(OnClearRow);
        EventBus.Unsubscribe<ClearColumnEvent>(OnClearColumn);
        EventBus.Unsubscribe<ClearRightDiagEvent>(OnClearRightDiag);
        EventBus.Unsubscribe<ClearLeftDiagEvent>(OnClearLeftDiag);
    }


    // ================================
    // Game State Event Handlers
    // ================================

    private void OnStartGame(StartGameEvent e)
    {
        ResetCells();

        background.gameObject.SetActive(true);
        cells.gameObject.SetActive(true);
    }

    private void OnExitGame(ExitGameEvent e)
    {
        background.gameObject.SetActive(true);
        cells.gameObject.SetActive(true);
    }

    private void OnGameOver(GameOverEvent e)
    {
        background.gameObject.SetActive(true);
        cells.gameObject.SetActive(true);
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


    // ================================
    // Board Controller Event Handlers
    // ================================

    private void OnSetCell(SetCellEvent e)
    {
        grid[e.Index.x, e.Index.y].SetColor(e.Color);
    }

    private void OnClearRow(ClearRowEvent e)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[e.Row, i].Pop();
        }
    }

    private void OnClearColumn(ClearColumnEvent e)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, e.Column].Pop();
        }
    }

    private void OnClearRightDiag(ClearRightDiagEvent e)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, i].Pop();
        }
    }

    private void OnClearLeftDiag(ClearLeftDiagEvent e)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[RowSize - 1 - i, i].Pop();
        }
    }


    // ================================
    // Private Methods
    // ================================

    // Initializes the grid cells using children in the scene view
    private void InitializeCells()
    {
        Cell[] childCells = cells.GetComponentsInChildren<Cell>();

        int expected = RowSize * RowSize;
        Debug.Assert(expected == childCells.Length, $"Expected {expected}, found {childCells.Length}");

        foreach (Cell cell in childCells)
        {
            Vector2Int index = cell.Index;

            Debug.Assert(index.x >= 0 && index.x < RowSize && index.y >= 0 && index.y <= RowSize,
                $"Index {index} is out of bounds");
            Debug.Assert(grid[index.x, index.y] == null, $"Duplicate cell at {index}");

            grid[index.x, index.y] = cell;
        }

        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                Debug.Assert(grid[x, y] != null, $"Missing cell at ({x}, {y})");
            }
        }
    }

    private void ResetCells()
    {
        foreach (Cell cell in grid)
        {
            cell.SetEmpty();
        }
    }

}
