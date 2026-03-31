using UnityEngine;
using System;

[RequireComponent(typeof(SpriteView))]
[RequireComponent(typeof(Animator))]
public class Cell : MonoBehaviour
{
    // ==================================================
    // Constants
    // ==================================================
    private const int Empty = 0;    // potentially make global enum in sprite database

    // ==================================================
    // Local Events
    // ==================================================
    public event Action<Cell> PopFinished;

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
    // Initialization
    // ==================================================
    
    /// <summary>
	/// Initialize the cell's row count and sprite.
	/// Deterministic.
	/// </summary>
	/// <param name="rows">Number of rows in the grid.</param>
    public void Initialize()    // leave this function for future additions, ie animations, sound effects
    {
        image = GetComponent<SpriteView>();
        image.Initialize();
        Radius = image.Radius;

        animator = GetComponent<Animator>();

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


    // ==================================================
    // Event Handlers
    // ==================================================

    private void OnAnimationComplete()
    {
        //Debug.Log($"completed animation on {gameObject.name}", this);

        SetEmpty();
        PopFinished?.Invoke(this);
    }
}
