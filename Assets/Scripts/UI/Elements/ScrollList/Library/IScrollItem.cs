using UnityEngine;

public interface IScrollItem<T>
{
    void Set(T data, int index);
}
