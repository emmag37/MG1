using UnityEngine;
using System.Collections.Generic;


public interface ISpriteDatabase
{
    Sprite GetSprite(CellColor color);
}

/// <summary>
/// Creates a sprite database that can be set and added to in the inspector.
/// Provides global access through Instance.
/// </summary>
/// <remarks> 0: empty, 1: color1, 2: color2, 3: color3, 4: color4, 5: color5, 6: color6, 7: wildcard
/// </ remarks>
[CreateAssetMenu(fileName = "SpriteDatabase", menuName = "Game/Sprite Database")]
public class SpriteDatabase : ScriptableObject, ISpriteDatabase
{
    // ================================
    // Inspector Fields
    // ================================
    [SerializeField] private Sprite[] sprites = new Sprite[GameConstants.NumberColors + 1]; // account for the empty sprite


    // ================================
    // Unity Lifecycle
    // ================================

    private void OnValidate()
    {
        // catch sprites not set
        for (int i = 0; i < sprites.Length; i++)
        {
            Debug.Assert(sprites[i] != null, $"Sprite not set in database at index {i}");
        }
    }

    // ================================
    // Public Methods
    // ================================

    /// <summary>
	/// Safe access to the sprite database.
	/// </summary>
	/// <param name="color">Color sprite to get</param>
	/// <returns>The sprite object associated with color</returns>
    public Sprite GetSprite(CellColor color)
    {
        int index = (int)color;
        Debug.Assert(index >= 0 && index < sprites.Length,
            $"Invalid index {index} for length {sprites.Length}");

        return sprites[index];
    }
}