using UnityEngine;

/// <summary>
/// Manages the color generation of players using the CellColor enum.
/// </summary>
public static class PlayerPicker
{
    // ================================
    // Public Methods
    // ================================

    // Calculates a color based on the built-in random generator.
    public static CellColor ChooseColor()
    {
        Debug.Assert(GameConstants.NumberColors >= 0, "[PlayerPicker] GameConstants.NumberColors is less than 0");

        return (CellColor)Random.Range(1, GameConstants.NumberColors + 1);  // 0 reserved, colors start at 1
    }

    // construct a more complicated algorithm to generate player colors in the future
}
