using UnityEngine;

[RequireComponent(typeof(SpriteView))]
[RequireComponent(typeof(Animator))]
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
    private CellColor color = CellColor.Empty;

    private Animator animator;

    // ==================================================
    // Unity Lifecycle Methods
    // ==================================================

    void Awake()
    {
        image = GetComponent<SpriteView>();
        Radius = image.Radius;

        animator = GetComponent<Animator>();
    }


    // ==================================================
    // Initialization
    // ==================================================
    
    /// <summary>
	/// Initialize the cell's row count and sprite.
	/// </summary>
	/// <param name="rows">Number of rows in the grid.</param>
    public void Initialize()    // leave this function for future additions, ie animations, sound effects
    {
        SetEmpty();
    }

    // ==================================================
    // Event Handlers
    // ==================================================

    public void OnAnimationComplete()
    {
        //Debug.Log($"completed animation on {gameObject.name}", this);

        SetEmpty();
    }

    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Sets the cell sprite to the given color.
	/// </summary>
	/// <param name="newColor">New color for the cell.</param>
    public void SetColor(CellColor newColor)
    {
        image.SetSprite(SpriteDatabase.Instance.GetSprite(newColor));     // validate in sprite database
        color = newColor;
    }

    /// <summary>
	/// Sets the cell sprite to the empty color.
	/// </summary>
    public void SetEmpty()
    {
        if (color == CellColor.Empty) return;

        SetColor(CellColor.Empty);
    }

    public void Pop()
    {
        animator.SetTrigger("PopCell");
    }
}
