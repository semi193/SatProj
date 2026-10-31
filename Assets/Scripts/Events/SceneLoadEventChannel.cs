using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneLoadEventChannel", menuName = "Events/Scene Load Event Channel")]
public class SceneLoadEventChannel : ScriptableObject
{
    public event Action<string> OnSceneLoadRequested;

    public void RaiseEvent(string sceneName)
    {
        OnSceneLoadRequested?.Invoke(sceneName);
    }
}