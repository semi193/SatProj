using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameUI : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private VoidEventChannel _victoryEvent;
    [SerializeField] private VoidEventChannel _defeatEvent;

    [Header("Panels")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;

    [Header("Buttons")]
    [SerializeField] private Button _victoryRestartButton;
    [SerializeField] private Button _victoryMenuButton;
    [SerializeField] private Button _defeatRestartButton;
    [SerializeField] private Button _defeatMenuButton;

    [Header("Settings")]
    [SerializeField] private float _delayBeforeShow = 1f;
    private bool _isGameEnded = false;

    private void OnEnable()
    {
        if (_victoryEvent != null) _victoryEvent.OnEventRaised += OnVictoryTriggered;
        if (_defeatEvent != null) _defeatEvent.OnEventRaised += OnDefeatTriggered;

        _victoryRestartButton.onClick.AddListener(RestartGame);
        _defeatRestartButton.onClick.AddListener(RestartGame);
        _victoryMenuButton.onClick.AddListener(GoToMainMenu);
        _defeatMenuButton.onClick.AddListener(GoToMainMenu);

        if (_victoryPanel != null) _victoryPanel.SetActive(false);
        if (_defeatPanel != null) _defeatPanel.SetActive(false);
    }

    private void OnDisable()
    {
        if (_victoryEvent != null) _victoryEvent.OnEventRaised -= OnVictoryTriggered;
        if (_defeatEvent != null) _defeatEvent.OnEventRaised -= OnDefeatTriggered;
    }

    private void OnVictoryTriggered()
    {
        if (_isGameEnded) return; 
        _isGameEnded = true;
        StartCoroutine(ShowPanelWithDelay(_victoryPanel));
    }

    private void OnDefeatTriggered()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;
        StartCoroutine(ShowPanelWithDelay(_defeatPanel));
    }

    private IEnumerator ShowPanelWithDelay(GameObject panel)
    {
        yield return new WaitForSecondsRealtime(_delayBeforeShow);

        if (panel != null)
            panel.SetActive(true);

        PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}