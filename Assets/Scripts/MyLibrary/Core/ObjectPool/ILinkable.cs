using UnityEngine;

public interface ILinkable<TSelf, TData> where TSelf : ILinkable<TSelf, TData>
{
    TSelf Next { get; set; }
    TSelf Prev { get; set; }

    void Initialize(TData data);
}
