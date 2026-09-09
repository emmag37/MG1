using UnityEngine;
using UnityEngine.UI;


public abstract class PopUpView : UIView<PopUpViewType>
{
    public override PopUpViewType Type => popUpType;
    
    [SerializeField] private PopUpViewType popUpType;
    [SerializeField] private Button exitButton;

    private UIButton exitUIButton;

    protected virtual void OnValidate()
    {
        Debug.Assert(exitButton != null, "[PopUpView] Null exit button");
    }

    protected virtual void Awake()
    {
        exitButton.onClick.AddListener(Host.PopView<PopUpViewType>);

        exitUIButton = UIButtonFactory.ClosePopUp<PopUpViewType>(exitButton, Host);
    }
}

