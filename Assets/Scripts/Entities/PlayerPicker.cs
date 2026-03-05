/**
 * Insert File Description
 * 
 */
using UnityEngine;

public class PlayerPicker
{
    // ================================
    // Public Types
    // ================================
    public struct Colors
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
    private Colors currentColors;

    // ================================
    // Constructors
    // ================================

    public PlayerPicker()
    {
        Reset();    // function also initializes the values
    }

    // reset for a new game
    public void Reset()
    {
        currentColors.color = ChooseColor(); // sets up the first color
        currentColors.nextColor = ChooseColor(); // sets up the next color
    }

    // ================================
    // Public Methods
    // ================================

    // only function that updates the color
    public Colors GetNewPlayerColors()
    {
        Colors colors = currentColors;              // return the current, non-updated state

        currentColors.color = currentColors.nextColor;
        currentColors.nextColor = ChooseColor();

        return colors;
    }

    // ================================
    // Private Methods
    // ================================


    // want to eventually create an algorithm so that this isn't just random
    private int ChooseColor()
    {
        return Random.Range(1, numColors + 1);  // 0 is reserved for the empty sprite, colors start at 1
    }
}
