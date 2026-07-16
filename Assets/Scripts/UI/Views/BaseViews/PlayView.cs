using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI view for the game play scene.
/// </summary>
public class PlayView : BaseView
{
    [SerializeField] private Button pauseButton;

    void Awake()
    {
        Debug.Log("button listener");
        pauseButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.Pause));
    }

    protected override void SetInfo(IRuntimeData data) { }
}