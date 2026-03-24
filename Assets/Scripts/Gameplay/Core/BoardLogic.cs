using System;

/// <summary>
/// Manages a representation of the board.
/// Respsonsible for row checking/clearing logic and point calculation.
/// </summary>
public class BoardLogic
{
    // ================================
    // Constants
    // ================================

    private const int RowSize = GameConstants.RowSize;
    private const CellColor Empty = CellColor.Empty;
    private const CellColor WildCard = CellColor.WildCard;

    // ================================
    // Public Types
    // ================================

    /// <summary>
    /// Result of placing a piece on the board.
    /// Contains line clear information, score, and whether or not the board is full.
    /// </summary>
    public struct PlayResult
    {
        public int Points;

        public bool ClearRow;
        public bool ClearCol;
        public bool ClearRDiag;
        public bool ClearLDiag;

        public bool FullBoard;

    }

    // ================================
    // Constants
    // ================================
    private const int NumSpots = RowSize * RowSize;

    // ================================
    // Private Fields
    // ================================

    private int[] rowCounts = new int[RowSize];
    private int[] colCounts = new int[RowSize];
    private int rDiagCount = 0;
    private int lDiagCount = 0;

    private int numSpotsFilled = 0;
    private CellColor[,] gridColors = new CellColor[RowSize, RowSize];


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Initializes the board for a game.
	/// </summary>
    public BoardLogic()
    {
        ResetColors();
    }

    /// <summary>
    /// Checks whether the index is within bounds and open.
    /// </summary>
    /// <param name="row">The row to check.</param>
    /// <param name="col">The column to check.</param>
    /// <returns>
    /// <c>true</c> if the index is valid; otherwise <c>false</c>
    /// </returns>
    public bool ValidCell(int row, int col)
    {
        bool valid =
            (row >= 0 && row < RowSize) &&
            (col >= 0 && col < RowSize) &&
            gridColors[row, col] == Empty;

        return valid;
    }

    /// <summary>
	/// Resets the board for a new game.
	/// </summary>
    public void ResetBoard()
    {
        for (int i = 0; i < RowSize; i++)
        {
            rowCounts[i] = 0;
            colCounts[i] = 0;
        }
        rDiagCount = 0;
        lDiagCount = 0;

        // Reset the grid
        numSpotsFilled = 0;
        ResetColors();
    }

    /// <summary>
	/// If the parameters are valid, adds the player to the board and
	/// checks the internal state for wins.
	/// </summary>
	/// <param name="row">Row of the player.</param>
	/// <param name="col">Column of the player.</param>
	/// <param name="color">Color of the player.</param>
	/// <param name="result">Contains information about the play.</param>
	/// <returns><c>true</c> if the index and color were valid; otherwise <c>false</c>.</returns>
    public bool TryPlacePlayer(int row, int col, CellColor color, out PlayResult result)
    {
        if (!ValidCell(row, col) || color == Empty)
        {
            result = new PlayResult();
            return false;
        }

        result = PlacePlayer(row, col, color);

        return true;
    }
   

    // ================================
    // Internal Methods - Testing Only
    // ================================

    internal CellColor GetCellColor(int row, int col)
    {
        return gridColors[row, col];
    }

    internal int GetSpotsFilled()
    {
        return numSpotsFilled;
    }


    // ================================
    // Private Methods
    // ================================

    // places the player on the board and sets play result values
    private PlayResult PlacePlayer(int row, int col, CellColor color)
    {
        PlayResult result = new PlayResult();

        // Add player to the board
        gridColors[row, col] = color;

        rowCounts[row]++;
        colCounts[col]++;
        if (row == col) rDiagCount++;
        if (RowSize - 1 - row == col) lDiagCount++;

        // Check for full and matching lines
        result.ClearRow =
            (rowCounts[row] == RowSize) &&
            LineColorsMatch(i => (row, i), color);

        result.ClearCol =
            (colCounts[col] == RowSize) &&
            LineColorsMatch(i => (i, col), color);

        result.ClearRDiag =
            (rDiagCount == RowSize) &&
            LineColorsMatch(i => (i, i), color);

        result.ClearLDiag =
            (lDiagCount == RowSize) &&
            LineColorsMatch(i => (RowSize - 1 - i, i), color);

        // Clear any filled lines
        if (result.ClearRow) ClearRow(row);
        if (result.ClearCol) ClearColumn(col);
        if (result.ClearRDiag) ClearRDiagonal();
        if (result.ClearLDiag) ClearLDiagonal();

        if (!result.ClearRow && !result.ClearCol && !result.ClearRDiag && !result.ClearLDiag) // no lines cleared
            numSpotsFilled++;

        result.Points = CalculatePoints(result);
        result.FullBoard = (numSpotsFilled == NumSpots);

        return result;
    }

    // sets grid colors to empty
    private void ResetColors()
    {
        for (int x = 0; x < RowSize; x++)
        {
            for (int y = 0; y < RowSize; y++)
            {
                gridColors[x, y] = Empty;
            }
        }
    }

    // sets the number of points scored in result
    private int CalculatePoints(PlayResult result)
    {
        int linesCleared =
            (result.ClearRow ? 1 : 0) +
            (result.ClearCol ? 1 : 0) +
            (result.ClearRDiag ? 1 : 0) +
            (result.ClearLDiag ? 1 : 0);

        int points = linesCleared * linesCleared * 5; // multiply by lines cleared again for the combo score
        return points;
    }

    // returns whether the line indicated by indexSelector is all color, assumes that the line is full
    private bool LineColorsMatch(Func<int, (int r, int c)> indexSelector, CellColor color)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            CellColor cellColor = gridColors[r, c];

            if (color == WildCard) color = cellColor;   // pick color to compare to if color is WildCard

            if (cellColor != color && cellColor != WildCard) return false;
        }

        return true;
    }

    // safe count decrementer, ensures count never goes below 0
    private int DecrementCount(int count)
    {
        if (count > 0) return count - 1;
        return 0;
    }

    // Line Clearing Helpers
    private void ClearRow(int row)
    {
        SetLineEmpty(i => (row, i));
        rowCounts[row] = 0;
        
        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearColumn(int col)
    {
        SetLineEmpty(i => (i, col));
        colCounts[col] = 0;

        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearRDiagonal()
    {
        SetLineEmpty(i => (i, i));
        rDiagCount = 0;

        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        lDiagCount = DecrementCount(lDiagCount);
    }
    private void ClearLDiagonal()
    {
        SetLineEmpty(i => (RowSize - 1 - i, i));
        lDiagCount = 0;

        for (int i = 0; i < RowSize; i++) colCounts[i] = DecrementCount(colCounts[i]);
        for (int i = 0; i < RowSize; i++) rowCounts[i] = DecrementCount(rowCounts[i]);
        rDiagCount = DecrementCount(rDiagCount);
    }

    private void SetLineEmpty(Func<int, (int r, int c)> indexSelector)
    {
        for (int i = 0; i < RowSize; i++)
        {
            var (r, c) = indexSelector(i);
            gridColors[r, c] = Empty;
        }

        numSpotsFilled -= 4;
    }
}
