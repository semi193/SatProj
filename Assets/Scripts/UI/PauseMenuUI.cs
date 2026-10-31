using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private VoidEventChannel _pauseToggledEvent;

    [Header("UI Elements")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;

    private void OnEnable()
    {
        _pauseToggledEvent.OnEventRaised += TogglePanel;
    }

    private void OnDisable()
    {
        _pauseToggledEvent.OnEventRaised -= TogglePanel;
    }

    private void Start()
    {
        _resumeButton.onClick.AddListener(OnResumeClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        _panel.SetActive(false);
    }

    private void TogglePanel()
    {
        _panel.SetActive(!_panel.activeSelf);
    }

    private void OnResumeClicked()
    {
        _pauseToggledEvent.RaiseEvent();
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}