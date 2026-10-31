using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private SceneLoadEventChannel _sceneLoadEvent;

    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    [Header("Settings")]
    [SerializeField] private string _gameSceneName = "SampleScene";

    private void Start()
    {
        _playButton.onClick.AddListener(OnPlayClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnPlayClicked()
    {
        _sceneLoadEvent.RaiseEvent(_gameSceneName);
    }

    private void OnQuitClicked()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}