using UnityEngine;

/// <summary>
/// Base class for all UI views.
/// </summary>
public abstract class UIView : MonoBehaviour
{
    protected UIManager UI => UIManager.Instance;

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}

public abstract class BaseUIView : UIView
{
    [SerializeField] private BaseViewType type;
    public BaseViewType Type => type;
}

public abstract class PopUpUIView : UIView
{
    [SerializeField] private PopUpViewType type;
    public PopUpViewType Type => type;
}

