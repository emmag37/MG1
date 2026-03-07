using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerPicker
{
    // ================================
    // Public Types
    // ================================
    /// <summary>
	/// 
	/// </summary>
    public struct PlayerColors
    {
        public int color;
        public int nextColor;
    }

    // ================================
    // Constants
    // ================================
    private const int numColors = 7;

    // ================================
    // Private Fields
    // ================================
    private PlayerColors currentColors;

    // ================================
    // Constructors
    // ================================
    /// <summary>
	/// 
	/// </summary>
    public PlayerPicker()
    {
        Reset();    // function also initializes the values
    }


    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
    public PlayerColors CalculateNewPlayerColors()
    {
        PlayerColors colors = currentColors;              // return the current, non-updated state

        currentColors.color = currentColors.nextColor;
        currentColors.nextColor = ChooseColor();

        return colors;
    }

    /// <summary>
	/// 
	/// </summary>
    public void Reset()
    {
        currentColors.color = ChooseColor(); // sets up the first color
        currentColors.nextColor = ChooseColor(); // sets up the next color
    }


    // ================================
    // Private Methods
    // ================================

    private int ChooseColor()
    {
        return Random.Range(1, numColors + 1);  // 0 is reserved for the empty sprite, colors start at 1
    }
}
