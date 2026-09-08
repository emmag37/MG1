using UnityEngine;
using System.Collections.Generic;

public interface IIncrementList<T>
{
    T Next();
    T Prev();
}

public class RingList<T> : IIncrementList<T>
{
    private List<T> list;
    private int index;

    public RingList(List<T> list, int startIndex = 0)
    {
        this.list = list;
        index = startIndex;
    }

    public void SetCurrentValue(T value)
    {
        index = list.IndexOf(value);
    }

    public T Next()
    {
        index = (index + 1) % list.Count;

        return list[index];
    }

    public T Prev()
    {
        index = (index - 1 + list.Count) % list.Count;
        return list[index];
    }
}
