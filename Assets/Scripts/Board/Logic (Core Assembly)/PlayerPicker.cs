using UnityEngine;


public static class PlayerPicker
{
    // calculates a color based on Unity's random number generator
    public static CellColor ChooseColor()
    {
        Debug.Assert(GameConstants.NumberColors >= 0, "[PlayerPicker] GameConstants.NumberColors is less than 0");

        return (CellColor)Random.Range(1, GameConstants.NumberColors + 1);  // 0 reserved, colors start at 1
    }
}
