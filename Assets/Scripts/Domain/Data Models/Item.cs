using UnityEngine;

public abstract class Item<T>
{
    protected T data;

    public Item(T data)
    {
        this.data = data;
    }

    public virtual void Set(T value)
    {
        data = value;
    }

    public T Get()
    {
        return data;
    }

}
