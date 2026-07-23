using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UIButton
{
    // private fields
    private Button button;
    private Action action;
    private IAudio audioService;
    private AudioType sound;

    // constructor
    public UIButton(Button button, Action action, AudioType sound = AudioType.Button)
    {
        this.button = button;
        this.action = action;
        this.sound = sound;

        audioService = ServiceLocator.Get<IAudio>();
        button.onClick.AddListener(Click);
    }

    public void Dispose() => button.onClick.RemoveListener(Click);

    // private methods
    private void Click()
    {
        audioService.PlaySoundEffect(AudioType.Button);
        action?.Invoke();
    }
}

public static class UIButtonFactory
{
    public static UIButton Navigate<TType>(Button button, IUIViewHost host, TType viewType) where TType : struct, Enum
        => new UIButton(button, () => host.PushView(viewType));

    public static UIButton ClosePopUp<TType>(Button button, IUIViewHost host) where TType : struct, Enum
        => new UIButton(button, host.PopView<TType>);

    public static UIButton EditInput(Button button, TMP_InputField inputField)
        => new UIButton(button, inputField.ActivateInputField);

    // web link action eventually
}

