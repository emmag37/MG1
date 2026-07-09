using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
public class Cell : MonoBehaviour
{
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
    public CellColor Color = CellColor.Empty;

    // ==================================================
    // Private Fields
    // ==================================================
    private int rowSize;    // what is this for?

    private SpriteRenderer spriteRenderer;

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
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        SetSprite(SpriteDatabase.Instance.GetSprite(newColor));     // validate in sprite database
        Color = newColor;
    }

    public void SetShadow()
    {
        SetSprite(SpriteDatabase.Instance.GetShadow(Color));
    }

    public void ResetShadow()
    {
        SetSprite(SpriteDatabase.Instance.GetSprite(Color));
    }

    /// <summary>
	/// Sets the cell sprite to the empty color.
	/// </summary>
    public void SetEmpty()
    {
        if (Color == CellColor.Empty) return;

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


    // private function - to take sprite view component off of this script (actually simpler)
    private void SetSprite(Sprite sprite)
    {
        Debug.Assert(sprite != null, "Attempted to set sprite to null");

        spriteRenderer.sprite = sprite;
    }
}
