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
        Assert.IsTrue(board.ValidCell(x, y, CellColor.Empty));

        // test 2: Negative
        x = -1;
        y = -1;
        Assert.IsFalse(board.ValidCell(x, y, CellColor.Empty));

        // test 3: Out of bounds
        x = RowSize * 2;
        y = RowSize * 2;
        Assert.IsFalse(board.ValidCell(x, y, CellColor.Empty));

        // test 5: Max edge case
        x = RowSize;
        y = RowSize;
        Assert.IsFalse(board.ValidCell(x, y, CellColor.Empty));
    }

    [Test, Category("Bounds")]
    public void TryPlace_Invalid_ReturnsFalse()
    {
        bool success = board.TryPlacePlayer(-1, 0, CellColor.Color1, out _);
        Assert.IsFalse(success);

        success = board.TryPlacePlayer(0, 0, CellColor.Empty, out _);
        Assert.IsFalse(success);
    }


    // ================================
    // Player Placement
    // ================================

    [Test, Category("Place players")]
    public void Place_Single_Player()
    {
        // test 1: Place valid player on empty board
        TestPlacePlayer(1, 0, 0, CellColor.Color1);

        // test 2: Check invalid cell on top of player
        Assert.IsFalse(board.ValidCell(0, 0, CellColor.Empty));
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

    [Test, Category("Place players")]
    public void Place_On_Occupied_Cell_Fails()
    {
        board.TryPlacePlayer(0, 0, CellColor.Color1, out _);

        bool success = board.TryPlacePlayer(0, 0, CellColor.Color2, out _);

        Assert.IsFalse(success);
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
        TestRowClear(x);


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
        TestColumnClear(y);
    }

    [Test, Category("Wins")]
    public void Single_Win_Right_Diagonal()
    {
        // test 1: filled for win
        CellColor color = CellColor.Color1;

        // fill the right diagonal
        int x;
        for (x = 0; x < RowSize - 1; x++)
        {
            board.TryPlacePlayer(x, x, color, out _);
        }

        BoardLogic.PlayResult win = TestWin(0, x, x, color);

        Assert.IsTrue(win.ClearRDiag);

        // check diagonal is clear
        for (x = 0; x < RowSize; x++)
        {
            Assert.AreEqual(CellColor.Empty, board.GetCellColor(x, x));
        }
    }

    [Test, Category("Wins")]
    public void Single_Win_Left_Diagonal()
    {
        // test 1: filled for win
        CellColor color = CellColor.Color1;

        // fill the left diagonal
        int x;
        for (x = 0; x < RowSize - 1; x++)
        {
            board.TryPlacePlayer(x, RowSize - 1 - x, color, out _);
        }

        BoardLogic.PlayResult win = TestWin(0, x, RowSize - 1 - x, color);

        Assert.IsTrue(win.ClearLDiag);

        // check diagonal is clear
        for (x = 0; x < RowSize; x++)
        {
            Assert.AreEqual(CellColor.Empty, board.GetCellColor(x, RowSize - 1 - x));
        }
    }

    [Test, Category("Wins")]
    public void Single_Win_WildCard()
    {
        // test 1: one wild card
        CellColor color = CellColor.Color1;
        int x = 0;

        FillRow(x, color);
        BoardLogic.PlayResult win1 = TestWin(0, x, RowSize - 1, CellColor.WildCard);

        Assert.IsTrue(win1.ClearRow);
        TestRowClear(x);


        // test 2: multiple wild cards
        x = 1;

        board.TryPlacePlayer(x, 0, color, out _);
        board.TryPlacePlayer(x, 1, CellColor.WildCard, out _);
        board.TryPlacePlayer(x, 2, color, out _);
        board.TryPlacePlayer(x, 3, color, out _);

        BoardLogic.PlayResult win2 = TestWin(0, x, RowSize - 1, CellColor.WildCard);

        Assert.IsTrue(win2.ClearRow);
        TestRowClear(x);


        // test 3: mixed row and wild card (loss)
        x = 2;

        board.TryPlacePlayer(x, 0, color, out _);
        board.TryPlacePlayer(x, 1, CellColor.Color2, out _);
        board.TryPlacePlayer(x, 2, color, out _);
        board.TryPlacePlayer(x, 3, color, out _);

        BoardLogic.PlayResult loss = TestPlacePlayer(RowSize, x, RowSize - 1, CellColor.WildCard);

        Assert.IsFalse(loss.ClearRow);
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
        TestRowClear(x);

        // same row again - new color
        CellColor color2 = CellColor.Color2;

        FillRow(x, color2);
        BoardLogic.PlayResult win2 = TestWin(0, x, RowSize - 1, color2);

        Assert.IsTrue(win2.ClearRow);
        TestRowClear(x);
    }

    [Test, Category("Wins")]
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

        TestRowClear(x);
        TestColumnClear(y);
    }

    [Test, Category("Wins")]
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

        Assert.IsTrue(win.ClearRow);
        Assert.IsTrue(win.ClearCol);

        TestRowClear(x);
        TestColumnClear(y);
    }

    // ================================
    // Game Over
    // ================================

    [Test, Category("Game Over")]
    public void Game_Over_And_Reset()
    {
        // test 1: game over is true
        FillRow(0, CellColor.Color1);
        FillRow(1, CellColor.Color2);
        FillRow(2, CellColor.Color1);
        FillRow(3, CellColor.Color2);
        FillRow(4, CellColor.Color1);
        FillColumn(4, CellColor.Color3);

        BoardLogic.PlayResult gameOver = TestPlacePlayer(RowSize * RowSize, 4, 4, CellColor.Color4);

        Assert.IsTrue(gameOver.FullBoard);

        // test 2: reset board works
        board.ResetBoard();

        for (int x = 0; x < RowSize; x++)
        {
            TestRowClear(x);
        }

        Assert.AreEqual(0, board.GetSpotsFilled());
    }

    [Test, Category("Game Over")]
    public void Almost_Game_Over()
    {
        FillRow(0, CellColor.Color1);
        FillRow(1, CellColor.Color2);
        FillRow(2, CellColor.Color1);
        FillRow(3, CellColor.Color2);
        FillRow(4, CellColor.Color1);
        FillColumn(4, CellColor.Color3);

        BoardLogic.PlayResult almostOver = TestWin(RowSize * (RowSize - 1), 4, 4, CellColor.Color3);
        TestColumnClear(4);

        Assert.IsTrue(almostOver.ClearCol);
        Assert.IsFalse(almostOver.FullBoard);
    }


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

        return result;
    }

    private void TestRowClear(int x)
    {
        for (int y = 0; y < RowSize; y++)
        {
            Assert.AreEqual(CellColor.Empty, board.GetCellColor(x, y));
        }
    }

    private void TestColumnClear(int y)
    {
        for (int x = 0; x < RowSize; x++)
        {
            Assert.AreEqual(CellColor.Empty, board.GetCellColor(x, y));
        }
    }

    // fills the first RowSize - 1 cells in a row
    private void FillRow(int x, CellColor color)
    {
        for (int y = 0; y < RowSize - 1; y++)
        {
            board.TryPlacePlayer(x, y, color, out _);
        }
    }

    // fills the first RowSize - 1 cells in a column
    private void FillColumn(int y, CellColor color)
    {
        for (int x = 0; x < RowSize - 1; x++)
        {
            board.TryPlacePlayer(x, y, color, out _);
        }
    }
}
