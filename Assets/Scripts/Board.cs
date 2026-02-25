using UnityEngine;

public class Board : MonoBehaviour
{
    // store the grid children here
    private Cell[,] grid = new Cell[5, 5];

    void Awake()
    {
        // access the cells from the game scene
        int index = 0;

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                grid[x, y] = transform.GetChild(index).GetComponent<Cell>();
                index++;
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        return grid[x, y];
    }

    public void SetFilled(Vector2Int pos, Sprite sprite, int num)
    {
        Debug.Log("set cell (" + pos.x + ", " + pos.y + ") to " + num); // this num should match with the random number logged previously

        // adjust the pos to match grid indices
        pos.x = (pos.x * -1) + 2;   // reverse row direction first
        pos.y += 2;

        Debug.Log("grid pos: (" + pos.x + ", " + pos.y + ")");

        // set the cell at pos
        grid[pos.x, pos.y].AssignSprite(sprite, num);
    }
}
