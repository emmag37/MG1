/**
 * Insert File Description
 * 
 */
using UnityEngine;

public class PlayerPicker
{
    // private variables
    private int color;     // holds the value of the current color
    private int nextColor; // holds the value of the next color

    // constructor
    public PlayerPicker()
    {
        color = Random.Range(0, 7); // sets up the first color
        nextColor = Random.Range(0, 7); // sets up the next color
    }

    // only function that updates the color
    public int GetNewPlayerColor()
    {
        int current = color;

        color = nextColor;
        nextColor = Random.Range(0, 7);

        return current;
    }

    public int GetNextPlayerColor()
    {
        return nextColor;
    }

    // reset for a new game
    public void Reset()
    {
        color = Random.Range(0, 7); // sets up the first color
        nextColor = Random.Range(0, 7); // sets up the next color
    }
}
