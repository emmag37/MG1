using UnityEngine;

public class GridController : MonoBehaviour
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
    // Inspector Fields
    // ================================
    [SerializeField] private GameObject cells;

    // ================================
    // Private Fields
    // ================================
    private Cell[,] grid = new Cell[RowSize, RowSize];  // grid children


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void OnValidate()
    {
        Debug.Assert(cells != null, "Cells not set in cell grid controller");
    }


    // ================================
    // Initializers
    // ================================

    // Initializes the grid cells using children in the scene view
    public void Initialize()
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

        CellRadius = grid[0, 0].Radius;
    }


    // ================================
    // Public Methods
    // ================================

    public void SetCell(int row, int col, CellColor color)
    {
        grid[row, col].SetColor(color);
    }

    /// <summary>
	/// Resets the board to empty cells.
	/// </summary>
    public void Reset()
    {
        foreach (Cell cell in grid)
        {
            cell.SetEmpty();
        }
    }

    // wins - bug for double animation in a combo
    public void ClearRow(int row)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[row, i].Pop();
        }
    }
    public void ClearColumn(int col)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, col].Pop();
        }
    }
    public void ClearRightDiagonal()
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, i].Pop();
        }
    }
    public void ClearLeftDiagonal()
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[RowSize - 1 - i, i].Pop();
        }
    }
}
