using UnityEngine;
using UnityEngine.UI;
using System;

// no need for factory, these only send a bool to some other method that is given

public class UIToggle
{
    // private fields
    private Slider slider;
    private Action<bool> action;
    private AudioType sound;
    private IAudio audioService;


    // constructor
    public UIToggle(Slider slider, Action<bool> action, AudioType sound = AudioType.Button)
    {
        this.slider = slider;
        this.action = action;
        this.sound = sound;

        audioService = ServiceLocator.Get<IAudio>();

        slider.wholeNumbers = true;
        slider.minValue = 0;
        slider.maxValue = 1;

        slider.onValueChanged.AddListener(Moved);
    }

    public void Dispose() => slider.onValueChanged.RemoveListener(Moved);

    private void Moved(float v)
    {
        audioService.PlaySoundEffect(sound);

        action.Invoke(v != 0);
    }
}

