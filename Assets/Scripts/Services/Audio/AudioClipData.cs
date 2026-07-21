using UnityEngine;

public enum AudioType
{
    PlacePlayer,
    PickupPlayer,
    Win,
    GameOver,
    Button,
    Transition,
    GameMusic,
    UIMusic
}

[CreateAssetMenu(fileName = "AudioClipData", menuName = "Scriptable Objects/Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    public AudioType type;
    public AudioClip clip;

    // any other useful fields
}
