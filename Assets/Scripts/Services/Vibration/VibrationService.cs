using UnityEngine;


public class VibrationService : IVibration
{
    private bool vibrationOn;

    // constructor
    public VibrationService(bool vibrationOn)
    {
        this.vibrationOn = vibrationOn;
    }

    // interface methods
    public void ShortVibration()
    {
        if (vibrationOn)
            Handheld.Vibrate();
    }

    public void SetVibrationOn(bool on)
    {
        vibrationOn = on;
    }

    public bool GetSettings()
    {
        return vibrationOn;
    }
}
