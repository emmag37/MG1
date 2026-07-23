using UnityEngine;
using UnityEngine.UI;
using System;

public class UIButton<TType> where TType : struct, Enum 
{
    // private fields
    private IUIViewHost host;
    private IAudio audioService;

    private Button button;
    private TType nextView;     // type of the view that this button navigates to

    // constructor
    public UIButton(IUIViewHost host, Button button, TType nextView)
    {
        this.host = host;
        this.button = button;
        this.nextView = nextView;

        audioService = ServiceLocator.Get<IAudio>();

        button.onClick.AddListener(Click);
    }

    // private methods
    private void Click()
    {
        audioService.PlaySoundEffect(AudioType.Button);

        host.PushView<TType>(nextView);
    }
}
