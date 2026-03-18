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
        int count = 0;

        // test 1: Place valid player on empty board
        int x = 0;
        int y = 0;
        CellColor color = CellColor.Color1;
        count++;

        BoardLogic.PlayResult result;
        bool success = board.TryPlacePlayer(x, y, color, out result);

        Assert.IsTrue(success);
        Assert.AreEqual(color, board.GetCellColor(x, y));
        Assert.AreEqual(count, board.GetSpotsFilled());

        // test 2: Check valid cell on top of player
        Assert.IsFalse(board.ValidCell(x, y));
    }

    
    [Test, Category("Place players")]
    public void Place_Multiple_Players()
    {

    }


    // ================================
    // Win Checking
    // ================================

}
