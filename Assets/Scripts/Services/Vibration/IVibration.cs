using UnityEngine;

public interface IVibration
{
    void ShortVibration();
    void SetVibrationOn(bool on);

    bool GetSettings();
}
