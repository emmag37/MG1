using UnityEngine;
using UnityEngine.UI;
using System;


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
        audioService.PlaySoundEffect(sound);
        action?.Invoke();
    }
}

