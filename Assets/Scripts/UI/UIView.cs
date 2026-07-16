using UnityEngine;
using UnityEngine.UI;

// new ui view so this compiles while i work it out
public abstract class UIView : MonoBehaviour
{
    protected UIManager Manager => UIManager.Instance;  // maybe just make this a static class?

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

