using UnityEngine;


public interface ISpriteDatabase
{
    Sprite GetSprite(int key);
    bool TryGetSprite(int key, out Sprite sprite);
}
