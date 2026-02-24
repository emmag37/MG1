using UnityEngine;

public class Board : MonoBehaviour
{
    // store the grid children here
    private Transform[,] grid = new Transform[5, 5];

    void Awake()
    {
        // access the cells from the game scene
        int index = 0;

        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                grid[x, y] = transform.GetChild(index);
                index++;
            }
        }
    }

    public Transform GetCell(int x, int y)
    {
        return grid[x, y];
    }

    public void SetFilled(int x, int y)
    {
        // set the spot as filled
    }
}
