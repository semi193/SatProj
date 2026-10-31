using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneLoadEventChannel _sceneLoadEvent;

    private void OnEnable()
    {
        if (_sceneLoadEvent != null)
            _sceneLoadEvent.OnSceneLoadRequested += LoadScene;
    }

    private void OnDisable()
    {
        if (_sceneLoadEvent != null)
            _sceneLoadEvent.OnSceneLoadRequested -= LoadScene;
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }
}