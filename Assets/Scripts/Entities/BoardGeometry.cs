using UnityEngine;

/// <summary>
/// Manages the index and coordinate calculations of the board.
/// </summary>
public class BoardGeometry
{
    // ==================================================
    // Private Fields
    // ==================================================
    private int rowSize;
    private float cellOffset;               
    private Vector3 originCellPos;


    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Calculates the cell offset and origin world coordinates.
	/// </summary>
	/// <param name="rows">Number of rows on the board.</param>
	/// <param name="cellRadius">Radius of a spot on the board.</param>
	/// <param name="board">Boundaries of box that holds the board.</param>
    public void Initialize(int rows, float cellRadius, Bounds board)
    {
        rowSize = rows;

        float boardLeft = board.min.x;
        float boardRight = board.max.x;
        float boardTop = board.max.y;

        float gridWidth = boardRight - boardLeft;
        float spacing = (gridWidth - cellRadius * (rowSize * 2)) / (rowSize + 1);

        cellOffset = cellRadius * 2 + spacing;
        originCellPos = new Vector3(boardRight - gridWidth / 2, boardTop - gridWidth / 2, 0);
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Calculates the board index from a position.
	/// </summary>
	/// <param name="position">Transform in world coordinates</param>
	/// <returns>The board index corresponding to the board array.</returns>
    public Vector2Int TransformToBoardIndex(Vector3 position)
    {
        Vector2Int gridIndex = new Vector2Int();    // intermediary where the origin is the center

        gridIndex.x = Mathf.RoundToInt((position.y - originCellPos.y) / cellOffset);
        gridIndex.y = Mathf.RoundToInt((position.x - originCellPos.x) / cellOffset);

        return GridToBoardIndex(gridIndex);
    }

    /// <summary>
	/// Calculates the world position of a cell.
	/// </summary>
	/// <param name="index">Board index of the cell.</param>
	/// <returns>The world positon of the cell.</returns>
    public Vector3 BoardIndexToTransform(Vector2Int index)
    {
        Vector3 newTransform = Vector3.zero;
        Vector2Int gridIndex = BoardToGridIndex(index);     // intermediary where the origin is the center

        newTransform.y = originCellPos.y + gridIndex.x * cellOffset;
        newTransform.x = originCellPos.x + gridIndex.y * cellOffset;

        return newTransform;
    }


    // ==================================================
    // Private Methods
    // ==================================================

    // Adjusts the rounded world grid index to a value useful for array access.
    private Vector2Int GridToBoardIndex(Vector2Int pos)
    {
        Vector2Int index = new Vector2Int();

        index.x = (rowSize - 1) / 2 - pos.x;   // reverse row direction first
        index.y = pos.y + (rowSize - 1) / 2;

        return index;
    }

    private Vector2Int BoardToGridIndex(Vector2Int pos)
    {
        Vector2Int index = new Vector2Int();

        index.x = (rowSize - 1) / 2 - pos.x;   // reverse row direction first
        index.y = pos.y - (rowSize - 1) / 2;

        return index;
    }
}
