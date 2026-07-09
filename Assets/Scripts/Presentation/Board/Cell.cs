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

        SetColor(CellColor.Empty);
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
        switch (newColor)
        {
            case CellColor.Shadow:
                spriteRenderer.sprite = SpriteDatabase.Instance.GetShadow(Color);
                break;

            case CellColor.ResetShadow:
                spriteRenderer.sprite = SpriteDatabase.Instance.GetSprite(Color);
                break;

            default:
                Color = newColor;
                spriteRenderer.sprite = SpriteDatabase.Instance.GetSprite(Color);
                break;
        }
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
        SetColor(CellColor.Empty);
        PopFinished?.Invoke(this);
    }
}
