using UnityEngine;
using UnityEngine.UI;

public enum PopUpViewType
{
    None,
    Pause,
    Settings,
    Profile,
    Leaderboard,
    ChooseAvatar,
    RestartGame,
    SkipTutorial
}

public abstract class PopUpView : UIView<PopUpViewType>
{
    public override PopUpViewType Type => popUpType;
    
    [SerializeField] private PopUpViewType popUpType;
    [SerializeField] private Button exitButton;

    protected virtual void OnValidate()
    {
        Debug.Assert(exitButton != null, "Exit button not set in pop up view");
    }

    protected virtual void Awake()
    {
        exitButton.onClick.AddListener(Host.PopView<PopUpViewType>);
    }
}

