using UnityEngine;
using System;

/// <summary>
/// Manages the gameplay board and its visual representation in the scene.
/// Bridges board logic and geometry with the rendered grid, handles player
/// placement, and clears completed lines.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Board : MonoBehaviour
{
    // ================================
    // Events
    // ================================
    
    /// <summary>
	/// Invoked when all board spots are filled with players.
	/// </summary>
    public event Action FullBoard;

    // ================================
    // Public Properties
    // ================================

    /// <summary>
	/// World coordinates of the board's boundaries.
	/// </summary>
    public Bounds BoardBounds { get; private set; }

    // ================================
    // Private Fields
    // ================================

    private BoardLogic logic;
    private BoardGeometry geometry;

    private Cell[,] grid = new Cell[GameConstants.RowSize, GameConstants.RowSize];  // grid children


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        BoardBounds = GetComponent<SpriteRenderer>().bounds;
        Debug.Assert(BoardBounds.size != Vector3.zero, "Invalid board bounds");

        InitializeGrid();

        logic = new BoardLogic();

        geometry = new BoardGeometry();
        geometry.Initialize(grid[0, 0].Radius, BoardBounds); // validate the radius in sprite view
    }


    // ================================
    // Initializers
    // ================================


    // ================================
    // Public Methods
    // ================================

    /// <summary>
    /// Sets cell to the player's image and clears completed lines.
    /// </summary>
    /// <param name="position">World position of the player to be added.</param>
    /// <param name="color">Color of the player to be added.</param>
    /// <returns>Points scored on the play.</returns>
    /// <remarks>
    /// Invokes <see cref="FullBoard"/> if the board becomes full.
    /// </remarks>
    public int RunPlay(Vector2Int index, CellColor color)
    {
        grid[index.x, index.y].SetColor(color);                       // render the player on the board

        BoardLogic.PlayResult result;
        if (logic.TryPlacePlayer(index.x, index.y, color, out result))     // run the board logic
        {
            Debug.LogError($"Ran play with invalid index or color: {index}, {color}");
        }

        if (result.FullBoard) FullBoard?.Invoke();                    // activate a game over

        // set full rows to empty cells
        if (result.ClearRow) ClearRow(index.x);
        if (result.ClearCol) ClearColumn(index.y);
        if (result.ClearRDiag) ClearRightDiagonal();
        if (result.ClearLDiag) ClearLeftDiagonal();

        return result.Points;
    }

    /// <summary>
    /// Attempts to calculate the nearest valid board position for the player.
    /// </summary>
    /// <param name="position">World position to evaluate.</param>
    /// <param name="newPosition">
    /// The corresponding board-aligned world position if the location is valid.
    /// </param>
	/// <param name="index">
    /// The corresponding board index if the location is valid.
    /// </param>
    /// <returns>
    /// <c>true</c> if the position maps to a valid board cell; otherwise <c>false</c>.
    /// </returns>
    public bool TryGetPlayerPosition(Vector3 position, out Vector3 newPosition, out Vector2Int index)
    {
        index = geometry.TransformToBoardIndex(position);

        if (!logic.ValidCell(index.x, index.y))
        {
            newPosition = Vector3.zero;
            return false;
        }

        newPosition = geometry.BoardIndexToTransform(index);
        return true;
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

        logic.ResetBoard(); // validate in logic
    }


    // ================================
    // Private Methods
    // ================================

    // Initializes the grid cells using children in the scene view
    private void InitializeGrid()
    {
        Cell[] cells = GetComponentsInChildren<Cell>();

        int expected = GameConstants.RowSize * GameConstants.RowSize;
        Debug.Assert(expected == cells.Length, $"Expected {expected}, found {cells.Length}");

        foreach (Cell cell in cells)
        {
            Vector2Int index = cell.Index;

            Debug.Assert(index.x >= 0 && index.x < GameConstants.RowSize && index.y >= 0 && index.y <= GameConstants.RowSize,
                $"Index {index} is out of bounds");
            Debug.Assert(grid[index.x, index.y] == null, $"Duplicate cell at {index}");

            grid[index.x, index.y] = cell;
        }

        for (int x = 0; x < GameConstants.RowSize; x++)
        {
            for (int y = 0; y < GameConstants.RowSize; y++)
            {
                Debug.Assert(grid[x, y] != null, $"Missing cell at ({x}, {y})");
            }
        }
    }


    // Helpers to reset the grid sprites
    private void ClearRow(int row)
    {
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            grid[row, i].SetEmpty();     // validate this in sprite database
        }
    }
    private void ClearColumn(int col)
    {
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            grid[i, col].SetEmpty();
        }
    }
    private void ClearRightDiagonal()
    {
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            grid[i, i].SetEmpty();
        }
    }
    private void ClearLeftDiagonal()
    {
        for (int i = 0; i < GameConstants.RowSize; i++)
        {
            grid[i, i].SetEmpty();
        }
    }
}
