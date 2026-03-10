using UnityEngine;

/// <summary>
/// Creates a sprite database that can be set and added to in the inspector. 
/// </summary>
[CreateAssetMenu(fileName = "SpriteDatabase", menuName = "Game/Sprite Database")]
public class SpriteDatabase : ScriptableObject
{
    public Sprite[] sprites;
}
