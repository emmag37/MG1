using UnityEngine;

/// <summary>
/// Base class for all UI views.
/// </summary>
public abstract class UIView : MonoBehaviour
{
    [SerializeField] private ViewType type;
    public ViewType Type => type;

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
