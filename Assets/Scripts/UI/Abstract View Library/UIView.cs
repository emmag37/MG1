using UnityEngine;
using UnityEngine.UI;
using System;


public abstract class UIView<TType> : MonoBehaviour where TType : struct, Enum
{
    protected IUIViewHost Host { get; private set; }


    public abstract TType Type { get; }

    public void Initialize(IUIViewHost host, IUIData initData)
    {
        Host = host;
        InitializeData(initData);
    }

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

    protected abstract void InitializeData(IUIData initData);   // init data is a reference to be accessed by set info to set values on show, update the ref to update the view
    protected abstract void SetInfo(IUIData data = null);
}
