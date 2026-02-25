using UnityEngine;

public class Cell : MonoBehaviour
{
    // Components
    private SpriteRenderer sr;
    private Sprite empty_sprite;
    private int color = -1;    // represents the color that the space is filled with, -1 means empty

    // Called before start
    void Awake()
    {
        // Cache values
        sr = GetComponent<SpriteRenderer>();
        empty_sprite = sr.sprite;
    }

    // Fills the cell with the new color
    public void AssignCell(Sprite new_sprite, int num)
    {
        sr.sprite = new_sprite;
        color = num;
    }

    // Returns the current state of the cell
    public int GetCellColor()
    {
        return color;
    }
}
