using UnityEngine;

[RequireComponent(typeof(SpriteView))]
public class Cell : MonoBehaviour
{
    // ==================================================
    // Constants
    // ==================================================
    private const int Empty = 0;    // potentially make global enum in sprite database

    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] Vector2Int index;

    // ==================================================
    // Public Properties
    // ==================================================
    public Vector2Int Index => index;
    public float Radius { get; private set; }

    // ==================================================
    // Private Fields
    // ==================================================
    private int rowSize;
    private SpriteView image;
    private int color = Empty;

    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void Awake()
    {
        Debug.Assert(index.x > 0 && index.y > 0, "Index must be non-negative");      

        image = GetComponent<SpriteView>();
        Radius = image.Radius;
    }


    // ==================================================
    // Initialization
    // ==================================================

    /// <summary>
	/// Initialize the cell's row count and sprite.
	/// </summary>
	/// <param name="rows">Number of rows in the grid.</param>
    public void Initialize(int rows)
    {
        rowSize = rows;
        Debug.Assert(index.x <= rowSize && index.y <= rowSize, "Invalid index for number of rows");

        SetEmpty();
    }


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Sets the cell sprite to the given color.
	/// </summary>
	/// <param name="newColor">New color for the cell.</param>
    public void SetColor(int newColor)
    {
        image.SetSprite(SpriteDatabase.Instance.sprites[newColor]);
        color = newColor;
    }

    /// <summary>
	/// Sets the cell sprite to the empty color.
	/// </summary>
    public void SetEmpty()
    {
        SetColor(Empty);
    }
}
