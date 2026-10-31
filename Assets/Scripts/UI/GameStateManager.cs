using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private VoidEventChannel _pauseToggledEvent;

    private bool _isPaused = false;

    private void OnEnable()
    {
        _pauseToggledEvent.OnEventRaised += TogglePause;
    }

    private void OnDisable()
    {
        _pauseToggledEvent.OnEventRaised -= TogglePause;
    }

    private void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}