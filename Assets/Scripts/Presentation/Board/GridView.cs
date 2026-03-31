using UnityEngine;
using System.Collections.Generic;

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
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlacePlayerEvent>(OnSetPlayerCell);
        EventBus.Unsubscribe<GhostPreviewEvent>(OnGhostPreview);
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

    public List<Cell> GetCellsToClear(Vector2Int index, bool row, bool col, bool rDiag, bool lDiag)
    {
        // make sure to not add the current player twice
        List<Cell> cells = new List<Cell>();

        if (row) AddRowMinusPlayer(cells, index);
        if (col) AddColMinusPlayer(cells, index);
        if (rDiag) AddRDiagMinusPlayer(cells, index);
        if (lDiag) AddLDiagMinusPlayer(cells, index);

        cells.Add(grid[index.x, index.y]);

        return cells;
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

    // ================================
    // Private Methods
    // ================================

    private void AddRowMinusPlayer(List<Cell> cells, Vector2Int index)
    {
        int x = index.x;
        for (int y = 0; y < RowSize; y++)
        {
            if (y != index.y)
                cells.Add(grid[x, y]);
        }
    }

    private void AddColMinusPlayer(List<Cell> cells, Vector2Int index)
    {
        int y = index.y;
        for (int x = 0; x < RowSize; x++)
        {
            if (x != index.x)
                cells.Add(grid[x, y]);
        }
    }

    private void AddRDiagMinusPlayer(List<Cell> cells, Vector2Int index)
    {
        for (int i = 0; i < RowSize; i++)
        {
            if (i != index.x)
                cells.Add(grid[i, i]);
        }
    }

    private void AddLDiagMinusPlayer(List<Cell> cells, Vector2Int index)
    {
        int offset = RowSize - 1;
        for (int i = 0; i < RowSize; i++)
        {
            if (offset - i != index.x)
                cells.Add(grid[offset - i, i]);
        }
    }
}
