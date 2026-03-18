using NUnit.Framework;

public class BoardLogicTests
{
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
        int x = 2;
        int y = 2;
        Assert.IsTrue(board.ValidCell(x, y));

        // test 2: Negative
        x = -1;
        y = -1;
        Assert.IsFalse(board.ValidCell(x, y));

        // test 3: Out of bounds
        x = GameConstants.RowSize * 2;
        y = GameConstants.RowSize * 2;
        Assert.IsFalse(board.ValidCell(x, y));

        // test 4: Min edge case
        x = 0;
        y = 0;
        Assert.IsFalse(board.ValidCell(x, y));

        // test 5: Max edge case
        x = GameConstants.RowSize;
        y = GameConstants.RowSize;
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
        TestPlacePlayer(count, 4, 4, CellColor.Color2);

        // player 3
        count++;
        TestPlacePlayer(count, 0, 4, CellColor.Color3);

        // player 4
        count++;
        TestPlacePlayer(count, 4, 0, CellColor.Color4);

        // player 5
        count++;
        TestPlacePlayer(count, 2, 2, CellColor.Color5);
    }


    // ================================
    // Singular Win Checking
    // ================================

    [Test, Category("Single Wins")]
    public void Single_Win_Row()
    {

    }

    [Test, Category("Single Wins")]
    public void Single_Win_Column()
    {

    }

    [Test, Category("Single Wins")]
    public void Single_Win_Right_Diagonal()
    {

    }

    [Test, Category("Single Wins")]
    public void Single_Win_Left_Diagonal()
    {

    }

    [Test, Category("Single Wins")]
    public void Single_Win_WildCard()
    {
        // place the wild card last for the win
    }


    // ================================
    // Multiple Win Checking
    // ================================

    [Test, Category("Multiple Wins")]
    public void Sequential_Wins()
    {
        // row

        // same row

        // column

        // same column
    }

    [Test, Category("Multiple Wins")]
    public void Combo_Win()
    {
        // row and column

        // row, column, right diagonal, and left diagonal
    }

    [Test, Category("Multiple Wins")]
    public void Combo_Win_WildCard()
    {
        // row and column same color

        // row and column different colors
    }

    // ================================
    // Game Over
    // ================================

    // test game overs

    // also test reset board

    // ================================
    // Helper Functions
    // ================================

    private void TestPlacePlayer(int count, int x, int y, CellColor color)
    {
        BoardLogic.PlayResult result;
        bool success = board.TryPlacePlayer(x, y, color, out result);

        Assert.IsTrue(success);
        Assert.AreEqual(color, board.GetCellColor(x, y));
        Assert.AreEqual(count, board.GetSpotsFilled());
    }
}
