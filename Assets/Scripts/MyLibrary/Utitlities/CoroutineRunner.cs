using UnityEngine;

// static singleton, so simple i think this is fine

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner instance;

    // lazy init
    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Coroutine Runner");     // create a hidden game object for this property to live on
                Object.DontDestroyOnLoad(go);                           // persistence across all scenes
                instance = go.AddComponent<CoroutineRunner>();
            }

            return instance;
        }
    }
}
