using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipData", menuName = "Scriptable Objects/Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    public AudioType type;
    public AudioClip clip;
}
