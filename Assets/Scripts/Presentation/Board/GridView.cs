using UnityEngine;

public class GridView : MonoBehaviour
{
    // ================================
    // Constants
    // ================================
    private const int RowSize = GameConstants.RowSize;

    // ================================
    // Public Fields
    // ================================

    public float CellRadius { get; private set; }

    // ================================
    // Private Fields
    // ================================

    private Cell[,] grid = new Cell[RowSize, RowSize];  // grid children


    // ================================
    // Unity Lifecycle
    // ================================

    void OnEnable()
    {
        EventBus.Subscribe<PlacePlayerEvent>(OnSetPlayerCell);
        EventBus.Subscribe<GhostPreviewEvent>(OnGhostPreview);

        EventBus.Subscribe<WinEvent>(OnClearLines);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlacePlayerEvent>(OnSetPlayerCell);
        EventBus.Unsubscribe<GhostPreviewEvent>(OnGhostPreview);

        EventBus.Unsubscribe<WinEvent>(OnClearLines);
    }

    // ================================
    // Initializer
    // ================================

    // Initializes the grid cells using children in the scene view
    public void Initialize()
    {
        // initialize the grid
        Cell[] childCells = GetComponentsInChildren<Cell>();

        int expected = RowSize * RowSize;
        Debug.Assert(expected == childCells.Length, $"Expected {expected}, found {childCells.Length}");

        foreach (Cell cell in childCells)
        {
            Vector2Int index = cell.Index;

            Debug.Assert(index.x >= 0 && index.x < RowSize && index.y >= 0 && index.y < RowSize,
                $"Index {index} is out of bounds");
            Debug.Assert(grid[index.x, index.y] == null, $"Duplicate cell at {index}");

            grid[index.x, index.y] = cell;
            grid[index.x, index.y].Initialize();
        }

        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                Debug.Assert(grid[x, y] != null, $"Missing cell at ({x}, {y})");
            }
        }

        // initialize values
        CellRadius = grid[0, 0].Radius;
    }

    // ================================
    // Public Methods
    // ================================
    
    public void SetCell(Vector2Int index, CellColor color)
    {
        grid[index.x, index.y].SetColor(color);
    }

    public void ResetCells()
    {
        foreach (Cell cell in grid)
        {
            cell.SetEmpty();
        }
    }
    

    // ================================
    // Event Handlers
    // ================================

    private void OnSetPlayerCell(PlacePlayerEvent e)
    {
        grid[e.Index.x, e.Index.y].SetColor(e.Color);
    }

    private void OnGhostPreview(GhostPreviewEvent e)
    {
        grid[e.Index.x, e.Index.y].SetColor(CellColor.Shadow);
    }

    private void OnClearLines(WinEvent e)
    {
        // make an animation function - run animation then remove the pieces
        // leaning towards making your animation coordinator then adjusting this to best fit that
        // could maybe create a queue of all cells to pop, or just an array

        if (e.Row != -1) ClearRow(e.Row);
        if (e.Column != -1) ClearColumn(e.Column);
        if (e.RightDiag) ClearRightDiag();
        if (e.LeftDiag) ClearLeftDiag();
    }


    // ================================
    // Private Methods
    // ================================

    private void ClearRow(int row)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[row, i].Pop();
        }
    }

    private void ClearColumn(int col)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, col].Pop();
        }
    }

    private void ClearRightDiag()
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, i].Pop();
        }
    }

    private void ClearLeftDiag()
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[RowSize - 1 - i, i].Pop();
        }
    }
}
