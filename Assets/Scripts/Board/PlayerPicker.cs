using UnityEngine;

/// <summary>
/// Manages the color generation of players using the CellColor enum.
/// </summary>
public class PlayerPicker
{
    // ================================
    // Private Fields
    // ================================
    private CellColor nextColor;


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Calculates the next player's color.
	/// </summary>
	/// <returns>The current player and next player's color.</returns>
    public PlayerColors CalculateNewPlayerColors()
    {
        if (nextColor == CellColor.Empty) nextColor = ChooseColor();

        PlayerColors newColors = new PlayerColors();              // return the current, non-updated state

        newColors.PlayerColor = nextColor;
        newColors.NextColor = ChooseColor();

        nextColor = newColors.NextColor;

        return newColors;
    }

    public void SetNextColor(CellColor color)
    {
        nextColor = color;
    }

    /// <summary>
    /// Chooses a new 'next' color for a fresh game.
    /// </summary>
    public void Reset()
    {
        nextColor = ChooseColor();
    }


    // ================================
    // Private Methods
    // ================================

    // Calculates a color based on the built-in random generator.
        // - want to make this more advanced to adjust to the game
        // - when you improve it, put it in its own script
    private CellColor ChooseColor()
    {
        return (CellColor)Random.Range(1, GameConstants.NumberColors + 1);  // 0 reserved, colors start at 1
    }
}
