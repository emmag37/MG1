using UnityEngine;
using UnityEngine.UI;

// clean-up: probably can remove un typed ui views, most require data

/// <summary>
/// Base class for all UI views.
/// </summary>
public abstract class UIView : MonoBehaviour
{
    protected UIManager Manager => UIManager.Instance;

    // show methods
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Show(object data)
    {
        Show(); // fallback for non-data views
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}

public abstract class BaseView : UIView
{
    [SerializeField] private BaseViewType type;
    public BaseViewType Type => type;
}

public abstract class PopUpView : UIView
{
    [SerializeField] private PopUpViewType type;
    public PopUpViewType Type => type;

    [SerializeField] private Button exitButton;

    protected virtual void OnValidate()
    {
        Debug.Assert(exitButton != null, "Exit button not set in pop up view");
    }

    protected virtual void Awake()
    {
        exitButton.onClick.AddListener(Manager.PopOverlay);
    }
}

// generics

public abstract class BaseView<T> : BaseView
{
    public virtual void Show(T data)
    {
        Show();
    }

    public override void Show(object data)
    {
        if (data is T typedData)
        {
            Show(typedData);
        }
        else
        {
            Show(); // fallback if wrong type
        }
    }

    public virtual void UpdateView(T data) { }
}

public abstract class PopUpView<T> : PopUpView
{
    public virtual void Show(T data)
    {
        Show();
    }

    public override void Show(object data)
    {
        if (data is T typedData)
        {
            Show(typedData);
        }
        else
        {
            Show(); // fallback if wrong type
        }
    }

    public virtual void UpdateOverlay(T data) { }
}
