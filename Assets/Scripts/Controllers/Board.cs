using UnityEngine;
using System;

/// <summary>
/// Manages the gameplay board and its visual representation in the scene.
/// Bridges board logic and geometry with the rendered grid, handles player
/// placement, and clears completed lines.
/// </summary>
public class Board : MonoBehaviour
{
    // ================================
    // Constants
    // ================================

    private const int RowSize = 5;

    // ================================
    // Events
    // ================================

    /// <summary>
	/// Invoked when all board spots are filled with players.
	/// </summary>
    public event Action BoardFull;

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

    private GamePieceImage[,] grid = new GamePieceImage[RowSize, RowSize];  // grid children


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        BoardBounds = GetComponent<SpriteRenderer>().bounds;

        int index = 0;
        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                // access the cells from the game scene
                grid[x, y] = transform.GetChild(index).GetComponent<GamePieceImage>();
                index++;
            }
        }

        logic = new BoardLogic();

        geometry = new BoardGeometry();
        geometry.Initialize(RowSize, grid[0, 0].Radius, BoardBounds);
    }


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
	/// Invokes <see cref="BoardFull"/> if the board becomes full.
	/// </remarks>
    public int RunPlay(Vector3 position, int color)
    {
        Vector2Int index = geometry.TransformToBoardIndex(position);        

        grid[index.x, index.y].SetSprite(color);                      // render the player on the board

        var result = logic.PlacePlayer(index, color);                 // run the play calculations
        if (result.FullBoard) BoardFull?.Invoke();                  // activate a game over

        // render empty sprites for full lines
        if (result.ClearRow) ClearGridLine(i => (index.x, i));
        if (result.ClearCol) ClearGridLine(i => (i, index.y));
        if (result.ClearRDiag) ClearGridLine(i => (i, i));
        if (result.ClearLDiag) ClearGridLine(i => (RowSize - 1 - i, i));

        return result.Points;
    }

    /// <summary>
    /// Attempts to calculate the nearest valid board position for the player.
    /// </summary>
    /// <param name="position">World position to evaluate.</param>
    /// <param name="newPosition">
    /// The corresponding board-aligned world position if the location is valid.
    /// </param>
    /// <returns>
    /// <c>true</c> if the position maps to a valid board cell; otherwise <c>false</c>.
    /// </returns>
    public bool TryGetPlayerPosition(Vector3 position, out Vector3 newPosition)
    {
        Vector2Int index = geometry.TransformToBoardIndex(position);

        if (!logic.ValidIndex(index.x, index.y))
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
        foreach (GamePieceImage image in grid)
        {
            image.ResetPiece();
        }

        logic.ResetBoard();
    }


    // ================================
    // Private Methods
    // ================================

    private void ClearGridLine(Func<int, (int r, int c)> indexSelector)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            grid[r, c].ResetPiece();
        }
    }
}
