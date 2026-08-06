using UnityEngine;
using UnityEngine.UI;
using System;


public abstract class UIView<TType> : MonoBehaviour where TType : struct, Enum
{
    protected IUIViewHost Host { get; private set; }

    public abstract TType Type { get; }

    public virtual void Initialize(IUIViewHost host) => Host = host;

    public virtual void Show(IUIData data = null)
    {
        gameObject.SetActive(true);
        SetInfo(data);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void UpdateView(IUIData data) => SetInfo(data);

    protected abstract void SetInfo(IUIData data = null);
}
