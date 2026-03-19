using UnityEngine;

/// <summary>
/// Manages the color generation of players using the CellColor enum.
/// </summary>
public class PlayerPicker
{
    // ================================
    // Public Types
    // ================================

    /// <summary>
	/// Contains the color value of the current player and the next player.
	/// </summary>
    public struct PlayerColors
    {
        public CellColor color;
        public CellColor nextColor;
    }

    // ================================
    // Private Fields
    // ================================
    private CellColor nextColor;

    // ================================
    // Constructors
    // ================================

    /// <summary>
	/// Chooses the first 'next' color of the game.
	/// </summary>
    public PlayerPicker()
    {
        nextColor = ChooseColor();
    }

    
    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Calculates the next player's color.
	/// </summary>
	/// <returns>The current player and next player's color.</returns>
    public PlayerColors CalculateNewPlayerColors()
    {
        PlayerColors newColors = new PlayerColors();              // return the current, non-updated state

        newColors.color = nextColor;
        newColors.nextColor = ChooseColor();

        nextColor = newColors.nextColor;

        return newColors;
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
        return (CellColor)Random.Range(1, GameConstants.NumberColors);  // 0 is reserved for the empty sprite, colors start at 1
    }
}
