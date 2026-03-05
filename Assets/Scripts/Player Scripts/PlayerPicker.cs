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
    // want to add a struct so that it returns both colors at once

    // ================================
    // Constants
    // ================================
    private const int numColors = 7;
    
    // ================================
    // Private Fields
    // ================================
    private int color;     // holds the value of the current color
    private int nextColor; // holds the value of the next color

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
        color = ChooseColor(); // sets up the first color
        nextColor = ChooseColor(); // sets up the next color
    }

    // ================================
    // Public Methods
    // ================================

    // only function that updates the color
    public int GetNewPlayerColor()
    {
        int current = color;

        color = nextColor;
        nextColor = ChooseColor();

        return current;
    }

    // combine this into get new player color
    public int GetNextPlayerColor()
    {
        return nextColor;
    }


    // ================================
    // Private Methods
    // ================================

    // want to eventually create an algorithm so that this isn't just random
    private int ChooseColor()
    {
        return Random.Range(1, numColors + 1);  // 0 is reserved for the empty sprite
    }
}
