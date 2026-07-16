using UnityEngine;
using UnityEngine.UI;
using System;

public abstract class UIView<TType> : MonoBehaviour where TType : struct, Enum
{
    protected UIManager Manager { get; private set; }

    public abstract TType Type { get; }

    public void Initialize(UIManager manager)
    {
        Manager = manager;
    }

    public virtual void Show(IRuntimeData data = null)
    {
        gameObject.SetActive(true);
        SetInfo(data);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void UpdateView(IRuntimeData data) => SetInfo(data);

    protected abstract void SetInfo(IRuntimeData data);
}
