using UnityEngine;


[CreateAssetMenu(fileName = "SpriteDatabase", menuName = "Scriptable Objects/Sprite Database")]
public class SpriteDatabase : KeyedDatabase<CellColor, Sprite>, ISpriteDatabase
{
    public Sprite GetSprite(int key)
    {
        if (TryGetValue((CellColor)key, out Sprite sprite))
            return sprite;

        Debug.LogError($"[SpriteDatabase] Invalid key {(CellColor)key} for sprite dictionary");
        return null;
    }

    public bool TryGetSprite(int key, out Sprite sprite) => TryGetValue((CellColor)key, out sprite);
}
