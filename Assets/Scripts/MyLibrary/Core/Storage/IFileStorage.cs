using UnityEngine;

public interface IFileStorage
{
    void Save<T>(string fileName, T data);
    T Load<T>(string fileName) where T : new();
}
