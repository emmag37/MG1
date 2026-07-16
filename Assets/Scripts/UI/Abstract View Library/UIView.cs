using UnityEngine;
using UnityEngine.UI;
using System;

// new ui view so this compiles while i work it out
public abstract class UIView<TType> : MonoBehaviour where TType : struct, Enum
{
    protected UIManager Manager => UIManager.Instance;

    public abstract TType Type { get; }

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



