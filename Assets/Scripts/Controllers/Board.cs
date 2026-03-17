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
    // Constants
    // ================================

    private const int RowSize = 5;
    private const int Empty = 0;    // put this as a global in sprite database?

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

    private SpriteView[,] grid = new SpriteView[RowSize, RowSize];  // grid children


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Awake()
    {
        BoardBounds = GetComponent<SpriteRenderer>().bounds;
        Debug.Assert(BoardBounds.size != Vector3.zero, "Invalid board bounds");

        // validate grid initialization
        int index = 0;
        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                // access the cells from the game scene
                grid[x, y] = transform.GetChild(index).GetComponent<SpriteView>();
                index++;
            }
        }
        Debug.Assert(grid[0, 0] != null, "Grid origin not initialized");

        logic = new BoardLogic();

        geometry = new BoardGeometry();
        geometry.Initialize(RowSize, grid[0, 0].Radius, BoardBounds); // validate the radius in sprite view
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
	/// Invokes <see cref="FullBoard"/> if the board becomes full.
	/// </remarks>
    public int RunPlay(Vector2Int index, int color)
    {
        Debug.Assert(logic.ValidIndex(index.x, index.y), $"Ran play with invalid index: ({index.x}, {index.y})");

        // validate color in the sprite database
        grid[index.x, index.y].SetSprite(SpriteDatabase.Instance.sprites[color]);                      // render the player on the board

        var result = logic.PlacePlayer(index, color);                 // run the play calculations, validate result in logic
        if (result.FullBoard) FullBoard?.Invoke();                  // activate a game over

        // render empty sprites for full lines
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
        index = geometry.TransformToBoardIndex(position);    // validate in geometry, assuming if incorrect (-1, -1)?

        if (!logic.ValidIndex(index.x, index.y))    // validate in logic
        {
            newPosition = Vector3.zero;
            return false;
        }

        newPosition = geometry.BoardIndexToTransform(index);    // validate in geometry
        return true;
    }

    /// <summary>
	/// Resets the board to empty cells.
	/// </summary>
    public void Reset()
    {
        foreach (SpriteView image in grid)
        {
            image.SetSprite(SpriteDatabase.Instance.sprites[Empty]);    // validate in sprite database
        }

        logic.ResetBoard(); // validate in logic
    }


    // ================================
    // Private Methods
    // ================================

    // Helpers to reset the grid sprites
    private void ClearRow(int row)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[row, i].SetSprite(SpriteDatabase.Instance.sprites[Empty]);     // validate this in sprite database
        }
    }
    private void ClearColumn(int col)
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, col].SetSprite(SpriteDatabase.Instance.sprites[Empty]);
        }
    }
    private void ClearRightDiagonal()
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, i].SetSprite(SpriteDatabase.Instance.sprites[Empty]);
        }
    }
    private void ClearLeftDiagonal()
    {
        for (int i = 0; i < RowSize; i++)
        {
            grid[i, i].SetSprite(SpriteDatabase.Instance.sprites[Empty]);
        }
    }
}
