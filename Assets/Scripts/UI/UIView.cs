using UnityEngine;
using UnityEngine.UI;

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

public abstract class BaseView : UIView
{
    [SerializeField] private BaseViewType type;
    public BaseViewType Type => type;

    public virtual void Show(ViewData data)
    {
        Show();
    }
}

public abstract class PopUpView : UIView
{
    [SerializeField] private PopUpViewType type;
    public PopUpViewType Type => type;

    [SerializeField] private Button exitButton;

    void OnValidate()
    {
        Debug.Assert(exitButton != null, "Exit button not set in pop up view");
    }

    public void OnExitClicked()
    {
        UI.PopOverlay();    // change this to pop the full stack
    }
}

