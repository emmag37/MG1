using UnityEngine;

public abstract class ItemView<T> : MonoBehaviour
{
    public abstract void Set(T data, int index);
}
