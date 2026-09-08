using UnityEngine;


public class VibrationService : IVibration
{
    // ==================================================
    // Private Fields
    // ==================================================
    private bool vibrationOn;

    // ==================================================
    // Constructor
    // ==================================================

    public VibrationService(bool vibrationOn)
    {
        this.vibrationOn = vibrationOn;
    }


    // ==================================================
    // Interface Methods
    // ==================================================

    public void ShortVibration()
    {
        if (vibrationOn)
            Handheld.Vibrate();
    }

    public void SetVibrationOn(bool on)
    {
        vibrationOn = on;
    }

    public bool GetSettings() => vibrationOn;
}
