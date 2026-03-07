/// <summary>
/// 
/// </summary>

using UnityEngine;

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
	/// 
	/// </summary>
	/// <param name="rows"></param>
	/// <param name="cellRadius"></param>
	/// <param name="board"></param>
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
	/// 
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
    public Vector2Int TransformToBoardIndex(Vector3 position)
    {
        Vector2Int gridIndex = new Vector2Int();

        gridIndex.x = Mathf.RoundToInt((position.y - originCellPos.y) / cellOffset);
        gridIndex.y = Mathf.RoundToInt((position.x - originCellPos.x) / cellOffset);

        return GridToBoardIndex(gridIndex);
    }

    /// <summary>
	/// 
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
    public Vector3 BoardIndexToTransform(Vector2Int index)
    {
        Vector3 newTransform = Vector3.zero;
        Vector2Int gridIndex = BoardToGridIndex(index);

        newTransform.y = originCellPos.y + gridIndex.x * cellOffset;
        newTransform.x = originCellPos.x + gridIndex.y * cellOffset;

        return newTransform;
    }


    // ==================================================
    // Private Methods
    // ==================================================
    
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
