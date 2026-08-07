using UnityEngine;

public interface IScrollItem<T>
{
    GameObject GameObject { get; }
    void Set(T data, int index);
}
