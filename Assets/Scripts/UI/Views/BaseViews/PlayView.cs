using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game play scene.
/// </summary>
public class PlayView : BaseView
{
    [SerializeField] private Button pauseButton;

    private UIButton pauseUIButton;

    void Awake()
    {
        pauseUIButton = UIButtonFactory.Navigate<PopUpViewType>(pauseButton, Host, PopUpViewType.Pause);
    }

    protected override void InitializeData(IUIData initData) { }
    protected override void SetInfo(IUIData data) { }
}