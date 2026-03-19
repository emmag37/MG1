using NUnit.Framework;

public class BoardLogicTests
{
    private const int RowSize = GameConstants.RowSize;

    private BoardLogic board;

    [SetUp]
    public void Setup()
    {
        board = new BoardLogic();
    }

    // ================================
    // Index Checking
    // ================================

    [Test, Category("Bounds")]
    public void Valid_Index_Bounds()
    {
        // test 1: In bounds
        int x = RowSize / 2;
        int y = RowSize / 2;
        Assert.IsTrue(board.ValidCell(x, y));

        // test 2: Negative
        x = -1;
        y = -1;
        Assert.IsFalse(board.ValidCell(x, y));

        // test 3: Out of bounds
        x = RowSize * 2;
        y = RowSize * 2;
        Assert.IsFalse(board.ValidCell(x, y));

        // test 4: Min edge case
        x = 0;
        y = 0;
        Assert.IsFalse(board.ValidCell(x, y));

        // test 5: Max edge case
        x = RowSize;
        y = RowSize;
        Assert.IsFalse(board.ValidCell(x, y));
    }


    // ================================
    // Player Placement
    // ================================

    [Test, Category("Place players")]
    public void Place_Single_Player()
    {
        // test 1: Place valid player on empty board
        TestPlacePlayer(0, 0, 0, CellColor.Color1);

        // test 2: Check invalid cell on top of player
        Assert.IsFalse(board.ValidCell(0, 0));
    }

    
    [Test, Category("Place players")]
    public void Place_Multiple_Players()
    {
        // test: scatter players around the board (no filled lines)
        int count = 0;

        // player 1
        count++;
        TestPlacePlayer(count, 0, 0, CellColor.Color1);

        // player 2
        count++;
        TestPlacePlayer(count, RowSize - 1, RowSize - 1, CellColor.Color2);

        // player 3
        count++;
        TestPlacePlayer(count, 0, RowSize - 1, CellColor.Color3);

        // player 4
        count++;
        TestPlacePlayer(count, RowSize - 1, 0, CellColor.Color4);

        // player 5
        count++;
        TestPlacePlayer(count, RowSize / 2, RowSize / 2, CellColor.Color5);
    }


    // ================================
    // Singular Win Checking
    // ================================

    [Test, Category("Wins")]
    public void Single_Win_Row()
    {
        // test 1: filled for win
        CellColor color = CellColor.Color1;
        int x = 0;

        FillRow(x, color);
        BoardLogic.PlayResult win = TestWin(0, x, RowSize - 1, color);

        Assert.IsTrue(win.ClearRow);


        // test 2: filled for loss
        x = 1;

        FillRow(x, color);
        BoardLogic.PlayResult loss = TestPlacePlayer(RowSize, x, RowSize - 1, CellColor.Color2);

        Assert.IsFalse(loss.ClearRow);

    }

    [Test, Category("Wins")]
    public void Single_Win_Column()
    {
        // test 1: filled for win
        CellColor color = CellColor.Color1;
        const int y = 0;

        FillColumn(y, color);
        BoardLogic.PlayResult win = TestWin(0, RowSize - 1, y, color);

        Assert.IsTrue(win.ClearCol);
    }

    [Test, Category("Wins")]
    public void Single_Win_Right_Diagonal()
    {
        // test 1: filled for win
        CellColor color = CellColor.Color1;

        // fill the right diagonal
        BoardLogic.PlayResult result;
        int x;
        for (x = 0; x < RowSize - 1; x++)
        {
            board.TryPlacePlayer(x, x, color, out result);
        }
        x++;

        BoardLogic.PlayResult win = TestWin(0, x, x, color);

        Assert.IsTrue(win.ClearRDiag);
    }

    [Test, Category("Wins")]
    public void Single_Win_Left_Diagonal()
    {
        // test 1: filled for win
        CellColor color = CellColor.Color1;

        // fill the left diagonal
        BoardLogic.PlayResult result;
        int x;
        for (x = 0; x < RowSize - 1; x++)
        {
            board.TryPlacePlayer(x, RowSize - 1 - x, color, out result);
        }
        x++;

        BoardLogic.PlayResult win = TestWin(0, x, RowSize - 1 - x, color);

        Assert.IsTrue(win.ClearLDiag);
    }

    [Test, Category("Wins")]
    public void Single_Win_WildCard()
    {
        // test 1: filled for win
        CellColor color = CellColor.Color1;
        int x = 0;

        FillRow(x, color);
        BoardLogic.PlayResult win = TestWin(0, x, RowSize - 1, CellColor.WildCard);

        Assert.IsTrue(win.ClearRow);
    }


    // ================================
    // Multiple Win Checking
    // ================================

    [Test, Category("Wins")]
    public void Sequential_Wins()
    {
        // test 1: same row twice
        int x = 0;
        CellColor color1 = CellColor.Color1;

        FillRow(x, color1);
        BoardLogic.PlayResult win1 = TestWin(0, x, RowSize - 1, color1);

        Assert.IsTrue(win1.ClearRow);

        // same row again - new color
        CellColor color2 = CellColor.Color2;

        FillRow(x, color2);
        BoardLogic.PlayResult win2 = TestWin(0, x, RowSize - 1, color2);

        Assert.IsTrue(win2.ClearRow);
    }

    [Test, Category("Multiple Wins")]
    public void Combo_Win()
    {
        int x = RowSize - 1;
        int y = RowSize - 1;
        CellColor color = CellColor.Color4;

        FillRow(x, color);
        FillColumn(y, color);

        BoardLogic.PlayResult win = TestWin(0, x, y, color);

        Assert.IsTrue(win.ClearRow);
        Assert.IsTrue(win.ClearCol);
    }

    [Test, Category("Multiple Wins")]
    public void Combo_Win_WildCard()
    {
        // row and column different colors
        int x = RowSize - 1;
        int y = RowSize - 1;

        CellColor rowColor = CellColor.Color5;
        CellColor colColor = CellColor.Color3;

        FillRow(x, rowColor);
        FillColumn(y, colColor);

        BoardLogic.PlayResult win = TestWin(0, x, y, CellColor.WildCard);
    }

    // ================================
    // Game Over
    // ================================

    // test game overs

    // also test reset board

    // ================================
    // Helper Functions
    // ================================

    private BoardLogic.PlayResult TestPlacePlayer(int count, int x, int y, CellColor color)
    {
        BoardLogic.PlayResult result;
        bool success = board.TryPlacePlayer(x, y, color, out result);

        Assert.IsTrue(success);
        Assert.AreEqual(color, board.GetCellColor(x, y));
        Assert.AreEqual(count, board.GetSpotsFilled());

        return result;
    }

    // places player and runs general asserts for all win types.
    // must test for specific row/col/etc after this runs.
    private BoardLogic.PlayResult TestWin(int count, int x, int y, CellColor color)
    {
        BoardLogic.PlayResult result;
        bool success = board.TryPlacePlayer(x, y, color, out result);

        Assert.IsTrue(success);
        Assert.AreEqual(count, board.GetSpotsFilled());
        Assert.AreEqual(CellColor.Empty, board.GetCellColor(x, y));

        return result;
    }

    // fills the first RowSize - 1 cells in a row
    private void FillRow(int x, CellColor color)
    {
        BoardLogic.PlayResult result;
        for (int y = 0; y < RowSize - 1; y++)
        {
            board.TryPlacePlayer(x, y, color, out result);
        }
    }

    // fills the first RowSize - 1 cells in a column
    private void FillColumn(int y, CellColor color)
    {
        BoardLogic.PlayResult result;
        for (int x = 0; x < RowSize - 1; x++)
        {
            board.TryPlacePlayer(x, y, color, out result);
        }
    }
}
